﻿using Jrd.DebSet;

namespace Jrd
{
    public static class H
    {
        public static void T(string t)
        {
            var pr = DebSetUI.DebSetText.text;
            if (pr == "")
            {
                pr += t;
            }
            else
            {
                pr += "\n" + t;
            }

            DebSetUI.DebSetText.text = pr;
        }
    }
}