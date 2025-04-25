using System.Globalization;
using Pheidippides.Domain;
using Pheidippides.DomainServices.Extensions;
using Pheidippides.ExternalServices;

namespace Pheidippides.DomainServices.Notifiers;

public class EmailNotifier(SmtpClient smtpClient) : INotifier
{
    public NotifierType NotifierType => NotifierType.Email;

    public async Task Notify(Incident incident, User[] users, CancellationToken cancellationToken)
    {
        await users.IndependentParallelForEachAsync(
            async (x, token) =>
            {
                ArgumentNullException.ThrowIfNull(x.Email);
                await smtpClient.SendMessage(
                    "Алерт. " + incident.Title, 
                    GetBody(incident),
                    x.Email,
                    token);
            },
            cancellationToken);
    }

    private static string GetBody(Incident incident)
        => $"""
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset="UTF-8">
                <title>Pheidippides - Уведомление об инциденте</title>
            </head>
            <body>
                <table width="100%" cellpadding="0" cellspacing="0" border="0" bgcolor="#f5e7c1">
                    <tr>
                        <td align="center" style="padding: 20px 0;">
                            <table width="600" cellpadding="20" cellspacing="0" border="0" bgcolor="#f0e0b6" style="border: 1px solid #d4c9a8;">
                                <tr>
                                    <td align="center">
                                        <font size="5" color="#8c2e0b" face="Georgia, serif"><i>Pheidippides</i></font>
                                        <hr width="150" color="#8c2e0b" size="1">
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right">
                                        <font color="#6d5c45" face="Georgia, serif"><i>{incident.CreatedAt.ToString(CultureInfo.CurrentCulture)}</i></font>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <font size="4" color="#5a3921" face="Georgia, serif">{incident.Title}</font>
                                        <hr width="100%" color="#c4b089" size="1">
                                    </td>
                                </tr>
                                <tr>
                                    <td bgcolor="#f8f1e0" style="border-left: 3px solid #8c2e0b; padding: 15px;">
                                        <font face="Georgia, serif">
                                            {incident.Description}
                                        </font>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" style="padding: 20px 0;">
                                        <table cellpadding="10" cellspacing="0" bgcolor="#8c2e0b" style="border-radius: 50%; border: 2px solid #5a3921;">
                                            <tr>
                                                <td align="center">
                                                    <font color="#f0e0b6" size="2" face="Georgia, serif">Pheidippides</b></font>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center">
                                        <font color="#6d5c45" size="2" face="Georgia, serif"><i>Да не оставит вас Гермес в час испытаний</i></font>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </body>
            </html>
            """;
}