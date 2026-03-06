using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using ScottPlot.Plottables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace WTools
{
    internal class MailTools
    {
        /*
        //使用者完成指派工作自動寄信
        const (
            user     = "assess-system@dasesing.com"

    password = "168As@!"
	host     = "192.168.1.5:25"
)

type unencryptedAuth struct {
            smtp.Auth
        }

        public (a unencryptedAuth) Start(server* smtp.ServerInfo)(string, []byte, error) {
s= *server

    s.TLS = true

    return a.Auth.Start(&s)
}

type SendMailContex struct {
        Mailtype string //html OR ''
        To       string
        Subject  string
        Body     string
}

public (c SendMailContex) SendMail() error {
            hp = strings.Split(host, ":")

    auth= unencryptedAuth
            {
                smtp.PlainAuth(
                    "",
                    user,
                    password,
                    hp[0],
            

                ),
	}
            content_type= "Content-Type: text/plain; charset=UTF-8"

    if c.Mailtype != "" {
                content_type = "Content-Type: text/" + c.Mailtype + "; charset=UTF-8"

    }

            msg= []byte("To: " + c.To + "\r\nFrom: " + user + ">\r\nSubject: " + c.Subject + "\r\n" + content_type + "\r\n\r\n" + c.Body)

    send_to= strings.Split(c.To, ";")

    err= newSendMail(host, auth, user, send_to, msg)

    if err != nil
            {
                fmt.Println("Send ", c.To, " mail error:", err)

    }
else
{
                fmt.Println("Send ", c.To, " mail success!")

    }
return err
        }

        public newSendMail(addr string, a smtp.Auth, from string, to[]string, msg[]byte) error {
            c, err = smtp.Dial(addr)

    if err != nil
            {
                return err

    }
            defer c.Close()

    if err = c.Hello("localhost"); err != nil
            {
                return err

    }
            err = c.Auth(a)

    if err != nil
            {
                return err

    }
if err = c.Mail(from); err != nil
            {
                fmt.Printf("mail\n")

        return err

    }
for _, addr = range to
            {
                if err = c.Rcpt(addr); err != nil {
                    return err
            
        }
            }
            w, err = c.Data()

    if err != nil
            {
                return err

    }
            _, err = w.Write(msg)
	if err != nil
            {
                return err

    }
            err = w.Close()

    if err != nil
            {
                return err

    }
return c.Quit()
}*/
    }
}
