/*************************************************************\\
 * File: xytools_debuq.cs
 * _xytools Libary (Schulman)
 * by _xxyy(Philipp N.)
 * xxyy98+schulman@gmail.com
 * http://xxyy.bplaced.net/
 *   
 * This program is free software: you can redistribute it
 *   and/or modify it under the terms of the GNU General
 *   Public License as published by the Free Software
 *   Foundation, either version 3 of the License, or
 *   (at your option) any later version.
 *   
 * This program is distributed in the hope that it will be
 *   useful, but WITHOUT ANY WARRANTY; without even the implied
 *   warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR
 *   PURPOSE.  See the GNU General Public License for more
 *   details.
 *   
 * You should have received a copy of the GNU General Public
 * License along with this program.  If not, see
 * <http://www.gnu.org/licenses/>.
\\*************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace xytools
{
    public enum CodeArea
    {
        LoadLessons,
        Splash,
        None
    }
    public static class D
    {
        public static void W(string msg,CodeArea ca = CodeArea.None){
            if (!IsCodeAreaDebugEnabled(ca)) return;
            Debug.WriteLine(msg);
        }
        public static void W(string msg, string add, CodeArea ca = CodeArea.None)
        {
            if (!IsCodeAreaDebugEnabled(ca)) return;
            Debug.WriteLine(msg,add);
        }
        public static void W(object msg, CodeArea ca = CodeArea.None)
        {
            if (!IsCodeAreaDebugEnabled(ca)) return;
            Debug.WriteLine(msg);
        }
        public static void W(string msg, string[] args, CodeArea ca = CodeArea.None)
        {
            if (!IsCodeAreaDebugEnabled(ca)) return;
            Debug.WriteLine(msg, args);
        }
        public static void W(string msg, object arg, CodeArea ca = CodeArea.None)
        {
            if (!IsCodeAreaDebugEnabled(ca)) return;
            Debug.WriteLine(msg, arg);
        }
        public static void W(object msg, string add, CodeArea ca = CodeArea.None)
        {
            if (!IsCodeAreaDebugEnabled(ca)) return;
            Debug.WriteLine(msg, add);
        }
        private static bool IsCodeAreaDebugEnabled(CodeArea ca)
        {
            /*if (!schulman.SchulMan.LoggingEnabled) return false;
            switch (ca)
            {
                case CodeArea.LoadLessons:
                    if (schulman.SchulMan.LogLessonLoading) return true;
                    else return false;
                case CodeArea.Splash:
                    return true;
                default:
                    return true;
            }*/
            return true;


        }
    }
}




