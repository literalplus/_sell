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
        public static void W(string msg, CodeArea ca = CodeArea.None)
        {
            if (!IsCodeAreaDebugEnabled(ca)) return;
            Debug.WriteLine(msg);
        }
        public static void W(string msg, string add, CodeArea ca = CodeArea.None)
        {
            if (!IsCodeAreaDebugEnabled(ca)) return;
            Debug.WriteLine(msg, add);
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




