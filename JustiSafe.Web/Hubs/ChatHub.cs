using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace JustiSafe.Web.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        // Ya NO recibimos el usuario como parámetro, lo calculamos aquí
        public async Task SendMessage(string message)
        {
            string displayName;

            // Lógica de Anonimización en el Servidor
            if (Context.User!.IsInRole("Admin"))
            {
                displayName = "🛡️ Soporte Técnico";
            }
            else
            {
                // Si es juez, ocultamos su nombre real
                displayName = "⚖️ Juez Anónimo";
            }

            // Enviamos 3 cosas: 
            // 1. El nombre falso (displayName)
            // 2. El mensaje
            // 3. El ID de conexión (Para que mi navegador sepa que fui YO quien lo envió)
            await Clients.All.SendAsync("ReceiveMessage", displayName, message, Context.ConnectionId);
        }
    }
}