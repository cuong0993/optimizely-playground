// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SendEmailWithAttachmentsAfterSubmissionActor.cs" company="Royal London">
//   @ RoyalLondon 2019
// </copyright>
// <summary>
//   The send email with attachments after submission actor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
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

namespace RoyalLondonGroup.CMS.Web.Business.Forms
{
    /// <summary>The send email with attachments after submission actor.</summary>
    public class SendEmailWithAttachmentsAfterSubmissionActor : SendEmailAfterSubmissionActor
    {
        /// <summary>The SMTP client.</summary>
        private static readonly SmtpClient smtpClient = new SmtpClient();

        /// <summary>Initializes a new instance of the <see cref="SendEmailWithAttachmentsAfterSubmissionActor" /> class.</summary>
        public SendEmailWithAttachmentsAfterSubmissionActor()
        {
            SendMessageInHtmlFormat = _formConfig.Service.SendMessageInHTMLFormat;
        }

        /// <summary>The is synced with submission process.</summary>
        public override bool IsSyncedWithSubmissionProcess => true;

        /// <summary>Gets a value indicating whether send message in html format.</summary>
        private bool SendMessageInHtmlFormat { get; }

        /// <summary>Gets the place holder service.</summary>
        private Injected<PlaceHolderService> PlaceHolderService { get; }

        /// <summary>The run.</summary>
        /// <param name="input">The input.</param>
        /// <returns>The <see cref="object" />.</returns>
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

        /// <summary>The send message.</summary>
        /// <param name="emailConfig">The email config.</param>
        private void SendMessage(EmailTemplateActorModel emailConfig)
        {
            // FormsHelper.RemoveFormOptionalFields(HttpContext.Current.Request.Cookies, this.FormIdentity.Guid, SubmissionData);           

            var toEmails = emailConfig.ToEmails;
            if (string.IsNullOrEmpty(toEmails))
            {
                _logger.Debug("There is no ToEmails to send. Skip.");
            }
            else
            {
                IEnumerable<PlaceHolder> bodyPlaceHolders = GetBodyPlaceHolders(null).ToList();
                _logger.Debug("Raw To field = {0}.", toEmails);
                var strArray = PlaceHolderService.Service.Replace(toEmails, bodyPlaceHolders, false)
                    .SplitBySeparators(new[] {",", ";", Environment.NewLine});

                if (strArray == null || !strArray.Any())
                    _logger.Debug("There is no ToEmails to send. Skip.");
                else
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
                        var emailHtmlTemplate = EmailHtmlTemplate(content);

                        var message = new MailMessage
                        {
                            Subject = str,
                            Body = RewriteUrls(emailHtmlTemplate),
                            IsBodyHtml = SendMessageInHtmlFormat
                        };
                        if (!string.IsNullOrEmpty(emailConfig.FromEmail))
                        {
                            _logger.Debug("Raw From field = {0}.", emailConfig.FromEmail);
                            placeHolderService = PlaceHolderService;
                            var address =
                                placeHolderService.Service.Replace(emailConfig.FromEmail, subjectPlaceHolders, false);
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
        }


        /// <summary>The attach uploaded files.</summary>
        /// <param name="message">The message.</param>
        /// <returns>The <see cref="MailMessage" />.</returns>
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
                            var attachment = new Attachment(postedFile.InputStream, postedFile.FileName, mime);
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

        /// <summary>Templated HTML email.</summary>
        /// <param name="content">The content to be templated as an email body.</param>
        /// <returns>The <see cref="string" />.</returns>
        private string EmailHtmlTemplate(string content)
        {
            return @"
                <body style='font-family: Arial, Helvetica, sans-serif;font-size: 14px;background: #FFFFFF;color: #333333;padding: 32px;margin: 0;line-height: 1.4;'>
                    <style type='text/css'>
                        html, body, div, span, applet, object, iframe,
                        h1, h2, h3, h4, h5, h6, p, blockquote, pre,
                        a, abbr, acronym, address, big, cite, code,
                        del, dfn, em, img, ins, kbd, q, s, samp,
                        small, strike, strong, sub, sup, tt, var,
                        b, u, i, center,
                        dl, dt, dd, ol, ul, li,
                        fieldset, form, label, legend,
                        table, caption, tbody, tfoot, thead, tr, th, td,
                        article, aside, canvas, details, embed, 
                        figure, figcaption, footer, header, hgroup {
	                        margin: 0;
	                        padding: 0;
	                        border: 0;
                            font-family: Arial, Helvetica, sans-serif;
	                        font-size: 15px;
	                        font: inherit;
                            line-height: 1.4;
	                        vertical-align: baseline;
                            color: #470054;
                        }

                        h1,h2,h3,h4,h5,h6 {
                            font-weight: bold;
                        }

                        h1 {font-size: 56px;}
                        h2 {font-size: 40px;}
                        h3 {font-size: 32px;}
                        h4 {font-size: 24px;}
                        h5 {font-size: 15px;}
                        h6 {font-size: 13px;}

                        a,
                        a:focus,
                        a:hover,
                        a:visited {
                            color: #470054;
                            text-decoration: underline;
                        }

                        .email-footer-team-name {
                            font-family: Georgia, serif;
                            font-weight: normal;
                            font-size: 24px;
                            color: #470054;
                        }

                        .email-footer-contact-details {
                            font-size: 13px;
                        }
                    </style>
                    " + content + @"
                </body>";
        }
    }
}