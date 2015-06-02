using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Dominion.Data.Authorization
{
    /// <summary>
    /// Send a code to Users predefined phone-number as the second part of two step verification
    /// </summary>
    class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            //TODO:  Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }
}
