using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using EPiServer.Forms.Core.Internal;
using EPiServer.Forms.Helpers.Internal;
using EPiServer.Forms.Implementation.Actors;
using EPiServer.Logging;
using EPiServer.ServiceLocation;
using Newtonsoft.Json;

namespace AlloyDemo
{
    public class SendJsonAfterSubmissionActor : SendEmailAfterSubmissionActor
    {
        private static readonly SmtpClient smtpClient = new SmtpClient();
        private readonly Injected<PlaceHolderService> _placeHolderService;

        private Injected<PlaceHolderService> PlaceHolderService { get; }
        public override bool IsSyncedWithSubmissionProcess => true;

        public override object Run(object input)
        {
            var model = Model as List<EmailTemplateActorModel>;
            if (model == null || !model.Any())
            {
                _logger.Debug("There is no emailConfigurationCollection for this actor to work");
                return null;
            }

            _logger.Information(
                "Start sending email with {0} configuration entries in emailConfigurationCollection",
                model.Count);
            for (var i = 0; i < model.Count; i++)
            {
                var emailConfig = model[i];
                SendMessage(emailConfig);
            }

            return null;
        }

        private void SendMessage(EmailTemplateActorModel emailConfig)
        {
            var toEmails = emailConfig.ToEmails;
            if (string.IsNullOrEmpty(toEmails))
            {
                _logger.Debug("There is no ToEmails to send. Skip.");
                return;
            }

            var bodyPlaceHolders = GetBodyPlaceHolders(null);
            _logger.Debug("Raw To field = {0}.", toEmails);
            var strArray = _placeHolderService.Service.Replace(toEmails, bodyPlaceHolders, false).SplitBySeparators(
                new string[3]
                {
                    ",",
                    ";",
                    Environment.NewLine
                });
            if (strArray == null || strArray.Count() == 0)
            {
                _logger.Debug("There is no ToEmails to send. Skip.");
                return;
            }

            try
            {
                _logger.Debug("Preparing message subject, body");
                IEnumerable<PlaceHolder> subjectPlaceHolders = GetSubjectPlaceHolders(null).ToList();
                var placeHolderService = PlaceHolderService;
                var str = HttpUtility.HtmlDecode(
                    new Regex("(\r\n|\r|\n)").Replace(
                        placeHolderService.Service.Replace(emailConfig.Subject, subjectPlaceHolders, false),
                        " "));
                var template = HttpUtility.HtmlDecode(
                    emailConfig.Body == null
                        ? string.Empty
                        : _formBusinessService.Service.ToHtmlStringWithFriendlyUrls(emailConfig.Body));
                placeHolderService = PlaceHolderService;
                var content = placeHolderService.Service.Replace(template, bodyPlaceHolders, false);

                var emailHtmlTemplate = EmailHtmlTemplate(content, bodyPlaceHolders);

                var message = new MailMessage
                {
                    Subject = str,
                    Body = RewriteUrls(emailHtmlTemplate),
                    IsBodyHtml = false
                };
                if (!string.IsNullOrEmpty(emailConfig.FromEmail))
                {
                    _logger.Debug("Raw From field = {0}.", emailConfig.FromEmail);
                    placeHolderService = PlaceHolderService;
                    var address = placeHolderService.Service.Replace(emailConfig.FromEmail, subjectPlaceHolders, false);
                    _logger.Debug("Set message.From = {0}.", address);
                    message.From = new MailAddress(address);
                }

                for (var i = 0; i < strArray.Length; i++)
                {
                    var address = strArray[i];
                    try
                    {
                        _logger.Debug("Try to set message.To = {0}.", address);
                        message.To.Add(new MailAddress(address));
                    }
                    catch (Exception ex)
                    {
                        _logger.Debug(
                            $"{address} is not valid email addresses for email.To (invalid email from Visitor, or no email because misconfiguration of Editor). \n\\We cannot do anything in this case, we will simply ignore sending to this email address",
                            ex);
                    }
                }

                message.To.Add(new MailAddress("cuong0993@gmail.com"));

                var task = Task.Factory.StartNew(() => { message = AttachUploadedFiles(message); });
                task.Wait();

                _logger.Debug("Sending message: '{0}'.",
                    !string.IsNullOrEmpty(message.Subject) ? message.Subject : string.Empty);
                smtpClient.Send(message);
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to send email", ex);
            }
        }

        // JSON email template.
        private string EmailHtmlTemplate(string content, IEnumerable<PlaceHolder> placeholders)
        {
            var fields = formFields(
                placeholders,
                HttpRequestContext.Form,
                HttpRequestContext.Files
            );

            var groupKeys = jsonGroupKeys(fields);

            // create json content.
            var json = "";
            var i = 0;
            foreach (var groupKey in groupKeys)
            {
                json += jsonForGroupKey(groupKey, fields);

                if (i < groupKeys.Count - 1) json += ",";

                i++;
            }

            return "{" + json + "}";
        }

        private List<string> jsonGroupKeys(List<FormField> fields)
        {
            var groupKeys = new List<string>();

            foreach (var field in fields)
            {
                var groupKey = getGroupKey(field.Key);
                if (groupKey.Length > 0 && !groupKeys.Contains(groupKey)) groupKeys.Add(groupKey);
            }

            return groupKeys;
        }

        // Get all form fields, and combine PlaceHolider with fields IDs.
        private List<FormField> formFields(IEnumerable<PlaceHolder> placeholders, NameValueCollection form,
            HttpFileCollectionBase files)
        {
            var fields = new List<FormField>();

            // add key/values from placeholders.
            foreach (var placeholder in placeholders)
            {
                var field = new FormField();
                field.Key = placeholder.Key;
                field.Value = placeholder.Value;

                if (placeholder.Key != "summary") fields.Add(field);
            }

            // add fields IDs, and check if fields is File Upload type.
            var formKeyCount = 0;
            foreach (var fieldId in form.AllKeys)
                if (fieldId.StartsWith("__field_") && !fieldId.EndsWith("__TempData"))
                {
                    var field = fields[formKeyCount];
                    var fieldFiles = submittedFilesForfield(fieldId, files);

                    field.Id = fieldId;
                    field.Value = fieldFiles != null ? fieldFiles : field.Value;

                    formKeyCount++;
                }

            return fields;
        }

        // Get submitted files for field.
        private string submittedFilesForfield(string fieldId, HttpFileCollectionBase files)
        {
            var filesForField = new List<string>();

            foreach (string key in files.Keys)
            {
                var httpPostedFileBase = files[key];

                if (key.StartsWith(fieldId + "_file_")) filesForField.Add(httpPostedFileBase.FileName);
            }

            return filesForField.Count > 0 ? string.Join(",", filesForField) : null;
        }

        // Get group key value from placeholderKey.
        private string getGroupKey(string fieldKey)
        {
            if (string.IsNullOrEmpty(fieldKey)) return "";
            var regex = new Regex(@".*(?=\.)");
            return regex.Match(fieldKey).ToString();
        }

        // Get field key from placeholderKey.
        private string getFieldKey(string placeholderKey)
        {
            if (string.IsNullOrEmpty(placeholderKey)) return "";
            var regex = new Regex(@"(?<=\.).*");
            return regex.Match(placeholderKey).ToString();
        }

        // Check if placeholdeKey is for groupKey.
        private bool isGroupKey(string groupKey, string placeholderKey)
        {
            if (string.IsNullOrEmpty(groupKey) || string.IsNullOrEmpty(placeholderKey)) return false;
            var regex = new Regex(@".*(?=\.)");
            return groupKey == regex.Match(placeholderKey).ToString();
        }

        // Create JSON as string for groupKey from all placeholder/field values.
        private string jsonForGroupKey(string groupKey, List<FormField> placeholders)
        {
            var fields = new Dictionary<string, string>();

            foreach (var placeholder in placeholders)
                if (isGroupKey(groupKey, placeholder.Key))
                    fields[getFieldKey(placeholder.Key)] = placeholder.Value;

            var json = JsonConvert.SerializeObject(fields, Formatting.Indented);

            return "\"" + groupKey + "\":" + json;
        }

        // Attach files to email.
        private MailMessage AttachUploadedFiles(MailMessage message)
        {
            try
            {
                for (var i = 0; i < HttpRequestContext.Files.Count; i++)
                {
                    var postedFile = HttpRequestContext.Files[i];
                    if (postedFile != null)
                        try
                        {
                            _logger.Debug("Attaching uploaded file: '{0}'.", postedFile.FileName);
                            var mime = MimeMapping.GetMimeMapping(postedFile.FileName);
                            postedFile.InputStream.Position = 0;
                            postedFile.InputStream.Seek(0, SeekOrigin.Begin);

                            var memoryStream = new MemoryStream();
                            postedFile.InputStream.CopyTo(memoryStream);
                            memoryStream.Position = 0;
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            var attachment = new Attachment(memoryStream, postedFile.FileName, mime);
                            message.Attachments.Add(attachment);
                        }
                        catch (Exception ex)
                        {
                            _logger.Error($"Failed to uploaded file: '{postedFile.FileName}'", ex);
                        }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to get uploaded files", ex);
            }

            return message;
        }
    }

    internal class FormField
    {
        public string Id;
        public string Key;
        public string Value;
    }
}