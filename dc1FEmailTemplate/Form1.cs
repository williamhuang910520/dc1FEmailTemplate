using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Mail;
using Outlook = Microsoft.Office.Interop.Outlook;
using Microsoft.CSharp;




namespace dc1FEmailTemplate
{
    public partial class Form1 : Form
    {
        

        public Form1()
        {
            InitializeComponent();
        }


        private void buttonGenerate_Click(object sender, EventArgs e)
        {
            String filePath = "\\\\140.125.155.18\\DataCenter\\110學年\\01-大廳活動\\自訂催繳格式.rtf";
            String debugPath = "C:\\Users\\Hanchi\\Desktop\\MyDocument.rtf"
;            if (name.Text == "")
            {
                MessageBox.Show("姓氏為空");
                return;
            }
            if (ckList_rentArea.CheckedItems.Count == 0)
            {
                MessageBox.Show("沒有選擇場地");
                return;
            }

            richTextBox1.LoadFile(debugPath);

            richTextBox1.Find("*姓氏*", RichTextBoxFinds.MatchCase);
            richTextBox1.SelectedText = name.Text;

            richTextBox1.Find("*開始日期*", RichTextBoxFinds.MatchCase);
            int minYear = int.Parse(startDatePicker.Value.ToString("yyyy"))-1911;
            richTextBox1.SelectedText = minYear.ToString() + startDatePicker.Value.ToString("/MM/dd");

            richTextBox1.Find("*結束日期*", RichTextBoxFinds.MatchCase);
            minYear = int.Parse(startDatePicker.Value.ToString("yyyy")) - 1911;
            richTextBox1.SelectedText = minYear.ToString() + endDatePicker.Value.ToString("/MM/dd");

            richTextBox1.Find("*借用場地*", RichTextBoxFinds.MatchCase);
            string s = ckList_rentArea.CheckedItems[0].ToString();
            for (int x = 1; x < ckList_rentArea.CheckedItems.Count; x++)
            {
                if(x == ckList_rentArea.CheckedItems.Count - 1)
                {
                    s += "及" + ckList_rentArea.CheckedItems[x].ToString();
                    break;
                }
                s += "、" + ckList_rentArea.CheckedItems[x].ToString();
            }
            richTextBox1.SelectedText = s;

            richTextBox1.Find("*是否逾時*", RichTextBoxFinds.MatchCase);
            richTextBox1.SelectedText = ckb_late.Checked?"由於逾時過久，所以":"";

            richTextBox1.Find("*歸還日期*", RichTextBoxFinds.MatchCase);
            DateTime date = endDatePicker.Value.AddDays(5);
            minYear = int.Parse(date.ToString("yyyy")) - 1911;
            richTextBox1.SelectedText = minYear.ToString() + date.ToString("/MM/dd");


        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            //Clipboard.SetText(richTextBox1.Rtf, TextDataFormat.Rtf);
            if(txbEmail.Text == "")
                MessageBox.Show("Email為空");
            else
                outlookSendMail(txbEmail.Text);
        }

        public void SendEmail()
        {
            //設定smtp主機
            string smtpAddress = "smtp.gmail.com";
            //設定Port
            int portNumber = 587;
            bool enableSSL = true;
            //填入寄送方email和密碼
            string emailFrom = "hanchitune76@gmail.com";
            string password = "hanchi_Tune";
            //收信方email
            string emailTo = "b10917013@gemail.yuntech.edu.tw";
            //主旨
            string subject = "Hello";
            //內容
            string body = "Hello, I'm just writing this to say Hi!";

            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(emailFrom);
                mail.To.Add(emailTo);
                mail.Subject = subject;
                mail.Body = body;
                // 若你的內容是HTML格式，則為True
                mail.IsBodyHtml = false;

                //夾帶檔案
                mail.Attachments.Add(new Attachment("C:\\Users\\Hanchi\\Desktop\\MyDocument.rtf"));

                using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                {
                    smtp.Credentials = new NetworkCredential(emailFrom, password);
                    smtp.EnableSsl = enableSSL;
                    smtp.Send(mail);
                }
            }
        }

        private void outlookSendMail(String emailAddress)
        {
            Outlook.Application application = new Outlook.Application();
            Outlook.MailItem mailItem = (Outlook.MailItem)application.CreateItem(Outlook.OlItemType.olMailItem);
            mailItem.To = emailAddress;
            mailItem.Subject = "I am Sub";
            var html = RtfPipe.Rtf.ToHtml(richTextBox1.Rtf);
            mailItem.HTMLBody = html;
            mailItem.Send();
            MessageBox.Show("已經發送郵件至" + emailAddress, "完成！");
        }
    }
}
