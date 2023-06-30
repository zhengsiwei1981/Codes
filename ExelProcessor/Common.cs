using System;
using System.Diagnostics;

namespace GJS.Infrastructure.Utility.NOPIFactory
{
    public class Common
    {
        /// 获取金额的大写中文文字            返回：中文数字文字
        ///  mvarOrDollar 数字金额大小, mstrLanguage 字符串语言 P：简体中文 C：繁体中文  
        /// </summary>
        /// <param name="mvarOrDollar"></param>
        /// <param name="mstrLanguage"></param>
        /// <returns></returns>
        public static string GetDollorStr(double mvarOrDollar)
        {
            //返回简体中文的中文描述
            return GetDollorStr(mvarOrDollar, "P");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mvarOrDollar"></param>
        /// <param name="mstrLanguage"></param>
        /// <returns></returns>
        public static string GetDollorStr(double mvarOrDollar, string mstrLanguage)
        {
            string t_word;
            string WLAMT;
            //            double tt;
            t_word = "";
            //            If mstrLanguage = "E" Or mstrLanguage = "e" Then
            //                t_word = t_word + noinword(Int(mvarOrDollar))
            //                If mvarOrDollar <> Int(mvarOrDollar) Then
            //                    tt = Int((mvarOrDollar - Int(mvarOrDollar)) * 100)
            //                    t_word = t_word & "And " & " Cents " & noinword(tt)
            //                End If
            //                 
            //            Else
            //            WLAMT = mvarOrDollar.ToString();
            WLAMT = StrFormat(mvarOrDollar, 12, 2);

            for (int i = 0; i < 12; i++)
            {
                if (i != 9)
                    t_word = t_word + SHRCHG(WLAMT, WLAMT.Substring(i, 1), i, mstrLanguage);

            }
            string spacestr = "";
            t_word = t_word + spacestr.PadLeft(40 - t_word.Length, ' ');

            //            End If
            return t_word.Trim();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="WLAMT"></param>
        /// <param name="WLCD"></param>
        /// <param name="WLLOC"></param>
        /// <param name="mstrLanguage"></param>
        /// <returns></returns>
        private static string SHRCHG(string WLAMT, string WLCD, int WLLOC, string mstrLanguage)
        {
            string WLNAME;
            string WLDD;
            if (mstrLanguage == "C")
                WLDD = "货ㄕ珺窾ㄕ珺じ  àだ";
            else
                //WLDD = "亿千百十万千百十元 角分";
                WLDD = "亿仟佰拾万仟佰拾元 角分";

            WLNAME = "    ";
            switch (WLCD)
            {
                case " ":
                    WLNAME = "   ";
                    break;
                case "1":
                    //WLNAME = IIf(mstrLanguage = "C", "滁", "壹") + Mid(WLDD, (WLLOC - 1) * 2 + 1, 2)
                    if (mstrLanguage.Equals("C"))
                        WLNAME = "滁" + WLDD.Substring(WLLOC, 1);
                    else
                        WLNAME = "壹" + WLDD.Substring(WLLOC, 1);
                    break;
                case "2":
                    //'WLNAME = IIf(mstrLanguage = "C", "禠", "贰") + Mid(WLDD, (WLLOC - 1) * 2 + 1, 2)
                    if (mstrLanguage.Equals("C"))
                        WLNAME = "禠" + WLDD.Substring(WLLOC, 1);
                    else
                        WLNAME = "贰" + WLDD.Substring(WLLOC, 1);

                    break;
                case "3":
                    //'WLNAME = IIf(mstrLanguage = "C", "把", "叁") + Mid(WLDD, (WLLOC - 1) * 2 + 1, 2)
                    if (mstrLanguage.Equals("C"))
                        WLNAME = "把" + WLDD.Substring(WLLOC, 1);
                    else
                        WLNAME = "叁" + WLDD.Substring(WLLOC, 1);

                    break;
                case "4":
                    //'WLNAME = IIf(mstrLanguage = "C", "竩", "肆") + Mid(WLDD, (WLLOC - 1) * 2 + 1, 2)
                    if (mstrLanguage.Equals("C"))
                        WLNAME = "竩" + WLDD.Substring(WLLOC, 1);
                    else
                        WLNAME = "肆" + WLDD.Substring(WLLOC, 1);

                    break;
                case "5":
                    //'WLNAME = IIf(mstrLanguage = "C", "ヮ", "伍") + Mid(WLDD, (WLLOC - 1) * 2 + 1, 2)
                    if (mstrLanguage.Equals("C"))
                        WLNAME = "ヮ" + WLDD.Substring(WLLOC, 1);
                    else
                        WLNAME = "伍" + WLDD.Substring(WLLOC, 1);

                    break;
                case "6":
                    //'WLNAME = IIf(mstrLanguage = "C", "嘲", "陆") + Mid(WLDD, (WLLOC - 1) * 2 + 1, 2)
                    if (mstrLanguage.Equals("C"))
                        WLNAME = "嘲" + WLDD.Substring(WLLOC, 1);
                    else
                        WLNAME = "陆" + WLDD.Substring(WLLOC, 1);

                    break;
                case "7":
                    //'WLNAME = IIf(mstrLanguage = "C", "琺", "柒") + Mid(WLDD, (WLLOC - 1) * 2 + 1, 2)
                    if (mstrLanguage.Equals("C"))
                        WLNAME = "琺" + WLDD.Substring(WLLOC, 1);
                    else
                        WLNAME = "柒" + WLDD.Substring(WLLOC, 1);

                    break;
                case "8":
                    //'WLNAME = IIf(mstrLanguage = "C", "", "捌") + Mid(WLDD, (WLLOC - 1) * 2 + 1, 2)
                    if (mstrLanguage.Equals("C"))
                        WLNAME = "" + WLDD.Substring(WLLOC, 1);
                    else
                        WLNAME = "捌" + WLDD.Substring(WLLOC, 1);
                    break;
                case "9":
                    //'WLNAME = IIf(mstrLanguage = "C", "╤", "玖") + Mid(WLDD, (WLLOC - 1) * 2 + 1, 2)
                    if (mstrLanguage.Equals("C"))
                        WLNAME = "╤" + WLDD.Substring(WLLOC, 1);
                    else
                        WLNAME = "玖" + WLDD.Substring(WLLOC, 1);
                    break;
                case "0":
                    string locList = "123567";
                    if (locList.IndexOf(WLLOC.ToString().Trim()) > 0 && WLAMT.Substring(WLLOC + 1, 1) != "0")
                        if (mstrLanguage.Equals("C"))
                            WLNAME = "箂";
                        else
                            WLNAME = "零";
                    else
                        WLNAME = "";
                    if (WLAMT.Substring(WLLOC, 1) == ".")
                        WLNAME = WLDD.Substring(WLLOC, 1);

                    if (WLLOC == 4 && (WLAMT.Substring(1, 1) != "0" || WLAMT.Substring(2, 1) != "0" || WLAMT.Substring(3, 1) != "0"))
                        if (mstrLanguage.Equals("C"))
                            WLNAME = "窾";
                        else
                            WLNAME = "万";
                    break;

            }
            return WLNAME.Trim();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Tlong"></param>
        /// <param name="Along"></param>
        /// <param name="Adec"></param>
        /// <returns></returns>
        private static string StrFormat(double Tlong, int Along, int Adec)
        {
            string tstr;


            tstr = Tlong.ToString().Trim();
            if (tstr.IndexOf(".") == -1)
            {
                tstr += ".00";
            }
            else
            {
                if (tstr.IndexOf(".") == 0)
                {
                    tstr = "0" + tstr;
                }
                if (tstr.IndexOf(".") == tstr.Length - 1)  //0.  case
                {
                    tstr = tstr + "0";
                }
                if (tstr.Substring(tstr.IndexOf(".") + 1).Length == 1)
                {
                    tstr = tstr + "0";
                }
                else
                {
                    tstr = tstr.Substring(0, tstr.IndexOf(".") + 3);
                }

            }
            if (tstr.Length < 12)
                tstr = tstr.PadLeft(12, ' ');
            return tstr;

        }
        /// <summary>
        /// soffice.exe执行pdf转换
        /// </summary>
        /// <param name="executePath"></param>
        /// <param name="wordPath"></param>
        /// <param name="pdfPath"></param>
        public static void WordConvertPDF(string executePath, string wordPath, string pdfPath)
        {
            ProcessStartInfo procStartInfo = new ProcessStartInfo(string.Format("\"{0}\"", executePath), string.Format(" --convert-to pdf --outdir \"{0}\" \"{1}\"", pdfPath, wordPath));
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = true;
            procStartInfo.WorkingDirectory = Environment.CurrentDirectory;

            Log.Debug(string.Format("\"{0}\"", executePath) + string.Format(" --convert-to pdf --outdir \"{0}\" \"{1}\"", pdfPath, wordPath));
            Process process = new Process() { StartInfo = procStartInfo, };
            process.Start();
            process.WaitForExit();
            // Check for failed exit code.
            if (process.ExitCode != 0)
            {
                throw new Exception(process.ExitCode.ToString());
            }
        }
    }
}
