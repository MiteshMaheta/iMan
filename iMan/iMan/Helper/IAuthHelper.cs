using System;
using System.Collections.Generic;
using System.Text;

namespace iMan.Helpers
{
    public interface IAuthHelper
    {
        void Authenticate(int requestCode = 0);
    }
}
