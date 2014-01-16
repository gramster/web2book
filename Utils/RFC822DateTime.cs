using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace web2book
{
    public class RFC2822DateTime
    /*
     * Copyright (c)  vendredi13@007.freesurf.fr
     * All rights reserved.
     *
     * Redistribution and use in source and binary forms, with or without
     * modification, are permitted.
     *
     * THIS SOFTWARE IS PROVIDED BY THE AUTHOR AND CONTRIBUTORS ``AS IS'' AND
     * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
     * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
     * ARE DISCLAIMED.  IN NO EVENT SHALL THE AUTHOR OR CONTRIBUTORS BE LIABLE
     * FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
     * DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS
     * OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
     * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
     * LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY
     * OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
     * SUCH DAMAGE.
     */
    {
        static public System.DateTime fromString(string adate)
        {
            string tmp;
            string[] resp;
            string dayName;
            string dpart;
            string hour, minute;
            string timeZone;
            System.DateTime dt = System.DateTime.Now;
            //--- strip comments
            //--- XXX : FIXME : how to handle nested comments ?
            tmp = Regex.Replace(adate, "(\\([^(].*\\))", "");

            // strip extra white spaces
            tmp = Regex.Replace(tmp, "\\s+", " ");
            tmp = Regex.Replace(tmp, "^\\s+", "");
            tmp = Regex.Replace(tmp, "\\s+$", "");

            // extract week name part
            resp = tmp.Split(new char[] { ',' }, 2);
            if (resp.Length == 2)
            {
                // there's week name
                dayName = resp[0];
                tmp = resp[1];
            }
            else dayName = "";

            try
            {
                // extract date and time
                int pos = tmp.LastIndexOf(" ");
                if (pos < 1) throw new FormatException("probably not a date");
                dpart = tmp.Substring(0, pos - 1);
                timeZone = tmp.Substring(pos + 1);
                dt = Convert.ToDateTime(dpart);

                // check weekDay name
                // this must be done befor convert to GMT 
                if (dayName != string.Empty)
                {
                    if ((dt.DayOfWeek == DayOfWeek.Friday && dayName != "Fri") ||
                        (dt.DayOfWeek == DayOfWeek.Monday && dayName != "Mon") ||
                        (dt.DayOfWeek == DayOfWeek.Saturday && dayName != "Sat") ||
                        (dt.DayOfWeek == DayOfWeek.Sunday && dayName != "Sun") ||
                        (dt.DayOfWeek == DayOfWeek.Thursday && dayName != "Thu") ||
                        (dt.DayOfWeek == DayOfWeek.Tuesday && dayName != "Tue") ||
                        (dt.DayOfWeek == DayOfWeek.Wednesday && dayName != "Wed")
                        )
                        throw new FormatException("Invalid day of week");
                }

                // adjust to localtime
                if (Regex.IsMatch(timeZone, "[+\\-][0-9][0-9][0-9][0-9]"))
                {
                    // it's a modern ANSI style timezone
                    int factor = 0;
                    hour = timeZone.Substring(1, 2);
                    minute = timeZone.Substring(3, 2);
                    if (timeZone.Substring(0, 1) == "+") factor = 1;
                    else if (timeZone.Substring(0, 1) == "-") factor = -1;
                    else throw new FormatException("Incorrect time zone");
                    dt = dt.AddHours(factor * Convert.ToInt32(hour));
                    dt = dt.AddMinutes(factor * Convert.ToInt32(minute));
                }
                else
                {
                    // it's a old style military time zone ?
                    switch (timeZone)
                    {
                        case "A": dt = dt.AddHours(1); break;
                        case "B": dt = dt.AddHours(2); break;
                        case "C": dt = dt.AddHours(3); break;
                        case "D": dt = dt.AddHours(4); break;
                        case "E": dt = dt.AddHours(5); break;
                        case "F": dt = dt.AddHours(6); break;
                        case "G": dt = dt.AddHours(7); break;
                        case "H": dt = dt.AddHours(8); break;
                        case "I": dt = dt.AddHours(9); break;
                        case "K": dt = dt.AddHours(10); break;
                        case "L": dt = dt.AddHours(11); break;
                        case "M": dt = dt.AddHours(12); break;
                        case "N": dt = dt.AddHours(-1); break;
                        case "O": dt = dt.AddHours(-2); break;
                        case "P": dt = dt.AddHours(-3); break;
                        case "Q": dt = dt.AddHours(-4); break;
                        case "R": dt = dt.AddHours(-5); break;
                        case "S": dt = dt.AddHours(-6); break;
                        case "T": dt = dt.AddHours(-7); break;
                        case "U": dt = dt.AddHours(-8); break;
                        case "V": dt = dt.AddHours(-9); break;
                        case "W": dt = dt.AddHours(-10); break;
                        case "X": dt = dt.AddHours(-11); break;
                        case "Y": dt = dt.AddHours(-12); break;
                        case "Z":
                        case "UT":
                        case "GMT": break;    // It's UTC
                        case "EST": dt = dt.AddHours(5); break;
                        case "EDT": dt = dt.AddHours(4); break;
                        case "CST": dt = dt.AddHours(6); break;
                        case "CDT": dt = dt.AddHours(5); break;
                        case "MST": dt = dt.AddHours(7); break;
                        case "MDT": dt = dt.AddHours(6); break;
                        case "PST": dt = dt.AddHours(8); break;
                        case "PDT": dt = dt.AddHours(7); break;
                        default: throw new FormatException("Invalid time zone");
                    }
                }
            }
            catch (Exception e)
            {
                throw new FormatException(string.Format("Invalid date:{0}:{1}", e.Message, adate));
            }
            return dt;
        }
    }
}

