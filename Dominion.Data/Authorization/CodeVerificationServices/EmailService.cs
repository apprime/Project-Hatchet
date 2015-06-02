using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Dominion.Data.Authorization
{
    /// <summary>
    /// Send a code to Users email address as the second part of two step verification
    /// </summary>
    class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            //TODO:  Plug in your email service here to send an email.
            return Task.FromResult(0);
        }
    }
}
