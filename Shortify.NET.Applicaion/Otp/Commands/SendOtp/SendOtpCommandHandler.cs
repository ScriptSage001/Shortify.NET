using Microsoft.Extensions.Options;
using Shortify.NET.Applicaion.Abstractions;
using Shortify.NET.Applicaion.Abstractions.Repositories;
using Shortify.NET.Applicaion.Helpers;
using Shortify.NET.Applicaion.Shared.Models;
using Shortify.NET.Common.FunctionalTypes;
using Shortify.NET.Common.Messaging.Abstractions;
using Shortify.NET.Core;
using System.Text;
using static Shortify.NET.Applicaion.Shared.Constant.EmailConstants;

namespace Shortify.NET.Applicaion.Otp.Commands.SendOtp
{
    internal sealed class SendOtpCommandHandler : ICommandHandler<SendOtpCommand>
    {
        private readonly IEmailServices _emailServices;

        private readonly IOtpRepository _otpRepository;

        private readonly IUnitOfWork _unitOfWork;

        private readonly int _otpLifeSpan;

        public SendOtpCommandHandler(
            IEmailServices emailServices, 
            IOptions<EmailSettings> options, 
            IOtpRepository otpRepository, 
            IUnitOfWork unitOfWork)
        {
            _emailServices = emailServices;
            _otpLifeSpan = options.Value.OtpLifeSpanInMinutes;
            _otpRepository = otpRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(SendOtpCommand command, CancellationToken cancellationToken = default)
        {
            var otp = GenerateOtp();
            var subject = string.Empty;
            var body = string.Empty;

            switch (command.OtpType)
            {
                case OtpType.VerifyEmail:
                    subject = Subject.VerifyEmailOtp;
                    body = PrepareEmailBodyForEmailVerification(otp);
                    break;

                case OtpType.ResetPassword:
                    subject = Subject.ResetPasswordOtp;
                    body = PrepareEmailBodyForPasswordReset(otp);
                    break;

                case OtpType.Login:
                    subject = Subject.LoginOtp;
                    body = PrepareEmailBodyForLogin(otp);
                    break;
            }

            MailRequest mailRequest = new
            (
                Recipients: [command.Email],
                Subject: subject,
                Body: body
            );

            await _emailServices.SendMailAsync(mailRequest, cancellationToken);

            await SaveOtpDetailsAsync(command.Email, otp, cancellationToken);

            return Result.Success();
        }

        #region Private Methods

        /// <summary>
        /// Generates Random 6 Digit Otp
        /// </summary>
        /// <returns></returns>
        private static string GenerateOtp()
        {
            Random random = new Random();

            return random.Next(100000, 1000000).ToString();
        }

        /// <summary>
        /// To Prepare the Email Body for VerifyEmail Scenario
        /// </summary>
        /// <param name="otp"></param>
        /// <returns></returns>
        private string PrepareEmailBodyForEmailVerification(string otp)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("<html lang='en'><head><style>body{font-family:Arial,sans-serif;line-height:1.6;background-color:#f4f4f4;margin:0;padding:20px;}");
            stringBuilder.Append(".container{max-width:600px;margin:auto;background:#fff;padding:20px;border-radius:5px;box-shadow:0 0 10px rgba(0,0,0,0.1);}");
            stringBuilder.Append(".btn{display:inline-block;background:#007bff;color:#fff;text-decoration:none;padding:10px 20px;border-radius:5px;}</style></head>");
            stringBuilder.Append("<body><div class='container'><h2>Email Verification</h2>");
            stringBuilder.Append("<p>Thank you for signing up with Shortify.NET! To verify your email address, please use the following OTP: </p><h3> {otp} </h3>");
            stringBuilder.Append("<br/><p>This OTP is valid for only {otpLifeSpan} minutes.</p>");
            stringBuilder.Append("<p>If you didn't sign up for an account with us, you can safely ignore this email.</p>");
            stringBuilder.Append("<br/><br/><p>Best Regards,<br>Shortify.NET Team</p></div></body></html>");

            var body = stringBuilder.ToString();
            body = body.Replace("{otp}", otp);
            body = body.Replace("{otpLifeSpan}", _otpLifeSpan.ToString());

            return body;
        }

        /// <summary>
        /// To Prepare the Email Body for VerifyEmail Scenario
        /// </summary>
        /// <param name="otp"></param>
        /// <returns></returns>
        private string PrepareEmailBodyForPasswordReset(string otp)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("<html lang='en'><head><style>body{font-family:Arial,sans-serif;line-height:1.6;background-color:#f4f4f4;margin:0;padding:20px;}");
            stringBuilder.Append(".container{max-width:600px;margin:auto;background:#fff;padding:20px;border-radius:5px;box-shadow:0 0 10px rgba(0,0,0,0.1);}");
            stringBuilder.Append(".btn{display:inline-block;background:#007bff;color:#fff;text-decoration:none;padding:10px 20px;border-radius:5px;}</style></head>");
            stringBuilder.Append("<body><div class='container'><h2>Forgot Password</h2>");
            stringBuilder.Append("<p>You recently requested to reset your password for your Shortify.NET account. Use the following OTP to reset your password: </p><h3> {otp} </h3>");
            stringBuilder.Append("<br/><p>This OTP is valid for only {otpLifeSpan} minutes.</p>");
            stringBuilder.Append("<p>If you didn't request a password reset, you can safely ignore this email.</p>");
            stringBuilder.Append("<br/><br/><p>Best Regards,<br>Shortify.NET Team</p></div></body></html>");

            var body = stringBuilder.ToString();
            body = body.Replace("{otp}", otp);
            body = body.Replace("{otpLifeSpan}", _otpLifeSpan.ToString());

            return body;
        }

        /// <summary>
        /// To Prepare the Email Body for Login Scenario
        /// </summary>
        /// <param name="otp"></param>
        /// <returns></returns>
        private string PrepareEmailBodyForLogin(string otp)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("<html lang='en'><head><style>body{font-family:Arial,sans-serif;line-height:1.6;background-color:#f4f4f4;margin:0;padding:20px;}");
            stringBuilder.Append(".container{max-width:600px;margin:auto;background:#fff;padding:20px;border-radius:5px;box-shadow:0 0 10px rgba(0,0,0,0.1);}");
            stringBuilder.Append(".btn{display:inline-block;background:#007bff;color:#fff;text-decoration:none;padding:10px 20px;border-radius:5px;}</style></head>");
            stringBuilder.Append("<body><div class='container'><h2>Login with OTP</h2>");
            stringBuilder.Append("<p>You are attempting to login to your Shortify.NET account. Please use the following OTP: </p><h3> {otp} </h3>");
            stringBuilder.Append("<br/><p>This OTP is valid for only {otpLifeSpan} minutes.</p>");
            stringBuilder.Append("<p>If you didn't request this OTP, please ignore this email.</p>");
            stringBuilder.Append("<br/><br/><p>Best Regards,<br>Shortify.NET Team</p></div></body></html>");

            var body = stringBuilder.ToString();
            body = body.Replace("{otp}", otp);
            body = body.Replace("{otpLifeSpan}", _otpLifeSpan.ToString());

            return body;
        }

        /// <summary>
        /// To Save OTP Details in DB
        /// </summary>
        /// <param name="email"></param>
        /// <param name="otp"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task SaveOtpDetailsAsync(
                                string email, 
                                string otp, 
                                CancellationToken cancellationToken = default)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken))
            {
                try
                {
                    await _otpRepository.AddOtpDetail(
                                            email,
                                            otp,
                                            DateTime.UtcNow,
                                            DateTime.UtcNow
                                                        .AddMinutes(_otpLifeSpan),
                                            cancellationToken);

                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    await transaction.CommitAsync(cancellationToken);
                }
                catch
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            }
            
        }

        #endregion
    }
}