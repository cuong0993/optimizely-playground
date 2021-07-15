using EPiServer.Forms.Core;
using EPiServer.Forms.Core.Internal;
using EPiServer.Forms.Core.Models;
using EPiServer.Forms.Core.Models.Internal;
using EPiServer.Forms.Helpers.Internal;
using EPiServer.Forms.Implementation.Actors;
using EPiServer.Logging;
using EPiServer.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using System.Web;

namespace AlloyDemo.Core.Infrastructure.Episerver.Forms
{

    public class SendEmailActor : SendEmailAfterSubmissionActor
    {
        private static SmtpClient _smtpClient = new SmtpClient();
        private bool _sendMessageInHTMLFormat = false;
        private readonly Injected<IFormRepository> _formRepository;

        public SendEmailActor()
        {
            _sendMessageInHTMLFormat = _formConfig.Service.SendMessageInHTMLFormat;
        }

        public override object Run(object input)
        {
            IEnumerable<EmailTemplateActorModel> model = this.Model as IEnumerable<EmailTemplateActorModel>;

            if (model == null || !model.Any())
            {
                return string.Empty;
            }

            foreach (EmailTemplateActorModel emailConfig in model)
            {
                SendMessage(emailConfig);

            }

            return null;
        }

        private void SendMessage(EmailTemplateActorModel emailConfig)
        {

            if (string.IsNullOrEmpty(emailConfig.ToEmails))
                return;
            string[] strArray = emailConfig.ToEmails.SplitBySeparator(",");
            if (strArray == null || ((IEnumerable<string>)strArray).Count<string>() == 0)
                return;
            try
            {
                IEnumerable<FriendlyNameInfo> friendlyNameInfos =
                    this._formRepository.Service.GetFriendlyNameInfos(this.FormIdentity, typeof(IExcludeInSubmission));
                IEnumerable<PlaceHolder> subjectPlaceHolders = this.GetSubjectPlaceHoldersCustom(friendlyNameInfos);
                IEnumerable<PlaceHolder> bodyPlaceHolders = this.GetBodyPlaceHoldersCustom(friendlyNameInfos);

                string str = "aaaaaaaaaaaaaaaaaaaaaaa";
                var body = emailConfig.Body != null ? emailConfig.Body.ToHtmlString() : string.Empty;
                string content = "bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb";
                MailMessage message = new MailMessage();
                message.Subject = str;
                message.Body = this.RewriteUrls(content);
                message.IsBodyHtml = this._sendMessageInHTMLFormat;


                if (!string.IsNullOrEmpty(emailConfig.FromEmail))
                {
                    MailMessage mailMessage = message;
                    MailAddress mailAddress =
                        new MailAddress("cuong0993@gmail.com");
                    mailMessage.From = mailAddress;
                }

                foreach (string template in strArray)
                {
                    try
                    {
                        MailAddressCollection to = message.To;
                        MailAddress mailAddress =
                            new MailAddress("cuong0993@gmail.com");
                        to.Add(mailAddress);
                    }
                    catch (Exception)
                    {
                    }
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
            List<PlaceHolder> placeHolderList = new List<PlaceHolder>();
            placeHolderList.AddRange(this.GetFriendlyNamePlaceHoldersCustom(this.HttpRequestContext, this.SubmissionData,
                friendlyNames, false));
            return placeHolderList;
        }

        protected virtual IEnumerable<PlaceHolder> GetBodyPlaceHoldersCustom(IEnumerable<FriendlyNameInfo> friendlyNames)
        {
            List<PlaceHolder> placeHolderList = new List<PlaceHolder>();
            placeHolderList.AddRange(this.GetPredefinedPlaceHoldersCustom(friendlyNames));
            placeHolderList.AddRange(this.GetFriendlyNamePlaceHoldersCustom(this.HttpRequestContext, this.SubmissionData,
                friendlyNames, this._sendMessageInHTMLFormat));
            return placeHolderList;
        }

        protected virtual IEnumerable<PlaceHolder> GetPredefinedPlaceHoldersCustom(
    IEnumerable<FriendlyNameInfo> friendlyNames)
        {
            return new List<PlaceHolder>()
            {
                new PlaceHolder("summary", this.GetFriendlySummaryTextCustom(friendlyNames))
            };
        }

        protected virtual string GetFriendlySummaryTextCustom(IEnumerable<FriendlyNameInfo> friendlyNames)
        {
            if (this.SubmissionData == null || this.SubmissionData.Data == null)
                return string.Empty;
            string separator = this._sendMessageInHTMLFormat ? "<br />" : Environment.NewLine;
            if (friendlyNames == null || friendlyNames.Count() == 0)
                return string.Join(separator,
                    this.SubmissionData.Data.Select(
                        x =>
                            string.Format("{0} : {1}", x.Key.ToLowerInvariant(),
                                string.IsNullOrEmpty(x.Value as string)
                                    ? string.Empty
                                    : WebUtility.HtmlEncode(x.Value as string))));
            return string.Join(separator,
                friendlyNames.Select(
                    x =>
                        string.Format("{0} : {1}", x.FriendlyName,
                            this.GetSubmissionDataFieldValueCustom(this.HttpRequestContext, this.SubmissionData, x, true))));
        }

        public virtual IEnumerable<PlaceHolder> GetFriendlyNamePlaceHoldersCustom(HttpRequestBase requestBase,
            Submission submissionData, IEnumerable<FriendlyNameInfo> friendlyNames, bool performHtmlEncode)
        {
            if (friendlyNames != null && friendlyNames.Count<FriendlyNameInfo>() != 0)
            {
                foreach (FriendlyNameInfo friendlyName in friendlyNames)
                {
                    FriendlyNameInfo fname = friendlyName;
                    yield return
                        new PlaceHolder(fname.FriendlyName,
                            this.GetSubmissionDataFieldValueCustom(requestBase, submissionData, fname, performHtmlEncode))
                        ;
                }
            }
        }

        public virtual string GetSubmissionDataFieldValueCustom(HttpRequestBase requestBase, Submission submissionData,
            FriendlyNameInfo friendlyName, bool performHtmlEncode)
        {
            string empty = string.Empty;
            if (submissionData == null || submissionData.Data == null)
                return empty;
            string rawData =
                submissionData.Data.FirstOrDefault(
                    x => x.Key.Equals(friendlyName.ElementId, StringComparison.OrdinalIgnoreCase)).Value as string;
            if (string.IsNullOrEmpty(rawData))
                return rawData;
            return performHtmlEncode ? WebUtility.HtmlEncode(rawData) : rawData;
        }
    }
}
