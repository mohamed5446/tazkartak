﻿using Tazkartk.Application.DTO;
using Tazkartk.Application.Interfaces.External;
namespace Tazkartk.Infrastructure;

public  class EmailBodyService : IEmailBodyService
{
    public  string ConfirmationEmailBody(string verificationCode)
    {
        return $@"
        <html lang='en'>
        <head>
            <meta charset='UTF-8' />
            <meta name='viewport' content='width=device-width, initial-scale=1.0' />
        </head>
        <body>
            <div style='background: linear-gradient(to right, #3c6b7e, #3a7285); padding: 20px; text-align: center;'>
                <h1 style='color: white;'>Verify Your Email</h1>
            </div>
            <div style='background-color: #f9f9f9; padding: 20px; border-radius: 0 0 5px 5px;'>
                <p>Hello,</p>
                <p>Thank you for signing up! Your verification code is:</p>
                <div style='text-align: center; margin: 30px 0;'>
                    <span style='font-size: 32px; font-weight: bold; letter-spacing: 5px; color: #3c6b7e;'>{verificationCode}</span>
                </div>
                <p>Enter this code on the verification page to complete your registration.</p>
                <p>This code will expire in 5 minutes for security reasons.</p>
                <p>If you didn't create an account with us, please ignore this email.</p>
                <p>Best regards,<br />Tazkartk Team</p>
            </div>
            <div style='text-align: center; margin-top: 20px; color: #888; font-size: 0.8em;'>
            </div>
        </body>
        </html>
        ";
    }
    public  string ResetPasswordEmailBody(string verificationCode)
    {
        return $@" <html>
                <head></head>
                <body style='font-family: Arial, sans-serif; color: #333;'>
                    <div style='background: linear-gradient(to right, #3c6b7e, #3a7285); padding: 20px; text-align: center;'>
                        <h1 style='color: white; margin: 0;'>Reset Your Password</h1>
                    </div>
                    <div style='background-color: #f9f9f9; padding: 20px; border-radius: 0 0 5px 5px; box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);'>
                        <p>Hello,</p>
                        <p>We received a request to reset your password. Your OTP for password reset is:</p>
                        <div style='text-align: center; margin: 30px 0'>
                            <span style='font-size: 32px; font-weight: bold; letter-spacing: 5px; color: #3c6b7e;'>{verificationCode}</span>
                        </div>
                        <p>Enter this code on the password reset page to reset your password.</p>
                        <p>This OTP will expire in 5 minutes for security reasons.</p>
                        <p>If you didn't request a password reset, please ignore this email.</p>
                        <p>Best regards,<br />Tazkartk Team</p>
                    </div>
                    <div style='text-align: center; margin-top: 20px; color: #888; font-size: 0.8em;'>
                    </div>
                </body>
            </html>";
    }
    public  string TripReminderEmailBody(TicketDTO Dto)
    {
        return $@"<!DOCTYPE html>
<html lang=""ar"" dir=""rtl"">
  <head>
    <meta charset=""UTF-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
    <title>تذكير بالرحلة</title>
  </head>
  <body
    style=""
      font-family: Arial, sans-serif;
      line-height: 1.6;
      color: #333;
      max-width: 600px;
      margin: 0 auto;
      padding: 20px;
    ""
  >
    <div
      style=""
        background: linear-gradient(to right, #3c6b7e, #3a7285);
        padding: 20px;
        text-align: center;
      ""
    >
      <h1 style=""color: white; margin: 0"">📢 تذكير برحلتك</h1>
    </div>
    <div
      style=""
        background-color: #f9f9f9;
        padding: 20px;
        border-radius: 0 0 5px 5px;
        box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
      ""
    >
      <p>مرحبًا <strong>{Dto.Name}</strong>،</p>
      <p>
        نود تذكيرك بأن رحلتك التي قمت بحجزها ستنطلق خلال ساعتين فقط.
      </p>
      <p>تفاصيل الرحلة:</p>
      <ul style=""line-height: 1.8;"">
        <li><strong>الشركة:</strong> {Dto.CompanyName}</li>
        <li><strong>التاريخ:</strong> {Dto.DepartureDate}</li>
        <li><strong>الوقت:</strong> {Dto.DepartureTime}</li>
        <li><strong>من:</strong> {Dto.From}</li>
        <li><strong>إلى:</strong> {Dto.To}</li>
        <li><strong>المقاعد:</strong> {string.Join("، ", Dto.SeatsNumbers)}</li>
      </ul>
      <p>يرجى التأكد من التواجد في مكان الانطلاق قبل الموعد بـ 15 دقيقة.</p>
      <p>نتمنى لك رحلة ممتعة وآمنة!</p>
      <p>مع تحياتنا،<br />فريق Tazkartk</p>
    </div>
    <div
      style=""
        text-align: center;
        margin-top: 20px;
        color: #888;
        font-size: 0.8em;
      ""
    >
      <p>تم إرسال هذه الرسالة تلقائيًا، الرجاء عدم الرد عليها.</p>
    </div>
  </body>
</html>
";
    }
    public string PayoutConfirmationEmailBody(string companyName, double amount, string date, string walletNumber)
    {
        return $@"
<!DOCTYPE html>
<html lang=""ar"" dir=""rtl"">
  <head>
    <meta charset=""UTF-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
    <title>تأكيد تحويل الأرباح</title>
  </head>
  <body style=""font-family: Arial, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: 0 auto; padding: 20px;"">
    <div style=""background: linear-gradient(to right, #3c6b7e, #3a7285); padding: 20px; text-align: center;"">
      <h1 style=""color: white; margin: 0;"">💰 تأكيد تحويل الأرباح</h1>
    </div>
    <div style=""background-color: #f9f9f9; padding: 20px; border-radius: 0 0 5px 5px; box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);"">
      <p>مرحبًا <strong>{companyName}</strong>،</p>
      <p>نود إعلامكم بأنه قد تم تحويل الأرباح الخاصة بكم بنجاح إلى المحفظة الإلكترونية.</p>
      <p>تفاصيل العملية:</p>
      <ul style=""line-height: 1.8;"">
        <li><strong>المبلغ:</strong> {amount:N2} جنيه مصري</li>
        <li><strong>التاريخ:</strong> {date}</li>
        <li><strong>المحفظة:</strong> {walletNumber}</li>
      </ul>
      <p>يرجى التأكد من استلام المبلغ من خلال تطبيق المحفظة.</p>
      <p>شكرًا لتعاونكم المستمر.</p>
      <p>مع تحياتنا،<br />فريق Tazkartk</p>
    </div>
    <div style=""text-align: center; margin-top: 20px; color: #888; font-size: 0.8em;"">
      <p>تم إرسال هذه الرسالة تلقائيًا، الرجاء عدم الرد عليها.</p>
    </div>
  </body>
</html>";
    }

    public string BalanceTransferEmailBody(string CompanyName,int TripId,string From,string To,string Date,double amount)
    {
        return $@"
    <html lang='ar' dir='rtl'>
    <body style='font-family: Cairo, Arial, sans-serif; line-height: 1.8; color: #333; max-width: 600px; margin: 0 auto; padding: 20px;'>
        <div style='background: linear-gradient(to right, #3c6b7e, #3a7285); padding: 20px; text-align: center;'>
            <h1 style='color: white; margin: 0;'>💼 تأكيد إضافة الأرباح</h1>
        </div>
        <div style='background-color: #f9f9f9; padding: 20px; border-radius: 0 0 5px 5px; box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);'>
            <p>مرحبًا <strong>{CompanyName}</strong>،</p>
            <p>يسعدنا إبلاغكم بأنه قد تم إضافة الأرباح الناتجة عن الرحلة التالية إلى رصيد حسابكم داخل تطبيق <strong>تذكرتك</strong>.</p>

            <p><strong>تفاصيل الرحلة:</strong></p>
            <ul>
                <li><strong>رقم الرحلة:</strong> {TripId}</li>
                <li><strong>من:</strong> {From}</li>
                <li><strong>إلى:</strong> {To}</li>
            </ul>

            <p><strong>تفاصيل العملية:</strong></p>
            <ul>
                <li><strong>المبلغ المضاف:</strong> {amount:N2} جنيه</li>
                <li><strong>تاريخ العملية:</strong> {Date:yyyy-MM-dd}</li>
            </ul>

            <p>يمكنكم طلب سحب الرصيد من خلال لوحة التحكم في أي وقت يناسبكم.</p>
            <p>شكرًا لثقتكم المستمرة بنا.</p>
            <p>مع تحيات،<br/>فريق <strong>تذكرتك</strong></p>
        </div>
        <div style='text-align: center; margin-top: 20px; color: #888; font-size: 0.8em;'>
            <p>هذا البريد مُرسل تلقائيًا، الرجاء عدم الرد عليه.</p>
        </div>
    </body>
    </html>";
    }
}
