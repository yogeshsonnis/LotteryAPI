using System;
using System.Threading.Tasks;

namespace LotteryNewAPI.Controllers
{
    internal class MessageDialog
    {
        private string v;

        public MessageDialog(string v)
        {
            this.v = v;
        }

        internal Task ShowAsync()
        {
            throw new NotImplementedException();
        }
    }
}