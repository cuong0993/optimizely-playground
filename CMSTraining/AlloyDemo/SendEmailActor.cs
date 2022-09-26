using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using EPiServer.Forms.Core;
using EPiServer.Forms.Core.Internal;
using EPiServer.Forms.Core.Models;
using EPiServer.Forms.Core.Models.Internal;
using EPiServer.Forms.Helpers.Internal;
using EPiServer.Forms.Implementation.Actors;
using EPiServer.Logging;
using EPiServer.ServiceLocation;

namespace AlloyDemo.Core.Infrastructure.Episerver.Forms
{
    public class SendEmailActor : SendEmailAfterSubmissionActor
    {
        private static readonly SmtpClient _smtpClient = new SmtpClient();
        private readonly Injected<IFormRepository> _formRepository;
        private readonly bool _sendMessageInHTMLFormat;

        public SendEmailActor()
        {
            _sendMessageInHTMLFormat = _formConfig.Service.SendMessageInHTMLFormat;
        }

        public override object Run(object input)
        {
            var model = Model as IEnumerable<EmailTemplateActorModel>;

            if (model == null || !model.Any()) return string.Empty;

            foreach (var emailConfig in model) SendMessage(emailConfig);

            return null;
        }

        private void SendMessage(EmailTemplateActorModel emailConfig)
        {
            if (string.IsNullOrEmpty(emailConfig.ToEmails))
                return;
            var strArray = emailConfig.ToEmails.SplitBySeparator();
            if (strArray == null || strArray.Count() == 0)
                return;
            try
            {
                var friendlyNameInfos =
                    _formRepository.Service.GetFriendlyNameInfos(FormIdentity, typeof(IExcludeInSubmission));
                var subjectPlaceHolders = GetSubjectPlaceHoldersCustom(friendlyNameInfos);
                var bodyPlaceHolders = GetBodyPlaceHoldersCustom(friendlyNameInfos);

                var str = "aaaaaaaaaaaaaaaaaaaaaaa";
                var body = emailConfig.Body != null ? emailConfig.Body.ToHtmlString() : string.Empty;
                var content = "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb";
                var message = new MailMessage();
                message.Subject = str;
                message.Body = RewriteUrls(content);
                message.IsBodyHtml = _sendMessageInHTMLFormat;


                if (!string.IsNullOrEmpty(emailConfig.FromEmail))
                {
                    var mailMessage = message;
                    var mailAddress =
                        new MailAddress("cuong0993@gmail.com");
                    mailMessage.From = mailAddress;
                }

                foreach (var template in strArray)
                    try
                    {
                        var to = message.To;
                        var mailAddress =
                            new MailAddress("cuong0993@gmail.com");
                        to.Add(mailAddress);
                    }
                    catch (Exception)
                    {
                    }

                _smtpClient.Send(message);
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to send e-mail:", ex);
            }
        }

        protected virtual IEnumerable<PlaceHolder> GetSubjectPlaceHoldersCustom(
            IEnumerable<FriendlyNameInfo> friendlyNames)
        {
            var placeHolderList = new List<PlaceHolder>();
            placeHolderList.AddRange(GetFriendlyNamePlaceHoldersCustom(HttpRequestContext, SubmissionData,
                friendlyNames, false));
            return placeHolderList;
        }

        protected virtual IEnumerable<PlaceHolder> GetBodyPlaceHoldersCustom(
            IEnumerable<FriendlyNameInfo> friendlyNames)
        {
            var placeHolderList = new List<PlaceHolder>();
            placeHolderList.AddRange(GetPredefinedPlaceHoldersCustom(friendlyNames));
            placeHolderList.AddRange(GetFriendlyNamePlaceHoldersCustom(HttpRequestContext, SubmissionData,
                friendlyNames, _sendMessageInHTMLFormat));
            return placeHolderList;
        }

        protected virtual IEnumerable<PlaceHolder> GetPredefinedPlaceHoldersCustom(
            IEnumerable<FriendlyNameInfo> friendlyNames)
        {
            return new List<PlaceHolder>
            {
                new PlaceHolder("summary", GetFriendlySummaryTextCustom(friendlyNames))
            };
        }

        protected virtual string GetFriendlySummaryTextCustom(IEnumerable<FriendlyNameInfo> friendlyNames)
        {
            if (SubmissionData == null || SubmissionData.Data == null)
                return string.Empty;
            var separator = _sendMessageInHTMLFormat ? "<br />" : Environment.NewLine;
            if (friendlyNames == null || friendlyNames.Count() == 0)
                return string.Join(separator,
                    SubmissionData.Data.Select(
                        x =>
                            string.Format("{0} : {1}", x.Key.ToLowerInvariant(),
                                string.IsNullOrEmpty(x.Value as string)
                                    ? string.Empty
                                    : WebUtility.HtmlEncode(x.Value as string))));
            return string.Join(separator,
                friendlyNames.Select(
                    x =>
                        string.Format("{0} : {1}", x.FriendlyName,
                            GetSubmissionDataFieldValueCustom(HttpRequestContext, SubmissionData, x, true))));
        }

        public virtual IEnumerable<PlaceHolder> GetFriendlyNamePlaceHoldersCustom(HttpRequestBase requestBase,
            Submission submissionData, IEnumerable<FriendlyNameInfo> friendlyNames, bool performHtmlEncode)
        {
            if (friendlyNames != null && friendlyNames.Count() != 0)
                foreach (var friendlyName in friendlyNames)
                {
                    var fname = friendlyName;
                    yield return
                        new PlaceHolder(fname.FriendlyName,
                            GetSubmissionDataFieldValueCustom(requestBase, submissionData, fname, performHtmlEncode))
                        ;
                }
        }

        public virtual string GetSubmissionDataFieldValueCustom(HttpRequestBase requestBase, Submission submissionData,
            FriendlyNameInfo friendlyName, bool performHtmlEncode)
        {
            var empty = string.Empty;
            if (submissionData == null || submissionData.Data == null)
                return empty;
            var rawData =
                submissionData.Data.FirstOrDefault(
                    x => x.Key.Equals(friendlyName.ElementId, StringComparison.OrdinalIgnoreCase)).Value as string;
            if (string.IsNullOrEmpty(rawData))
                return rawData;
            return performHtmlEncode ? WebUtility.HtmlEncode(rawData) : rawData;
        }
    }
}