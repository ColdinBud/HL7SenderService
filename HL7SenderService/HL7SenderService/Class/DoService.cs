using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HL7SenderService.Class
{
    class DoService
    {
        public static string pubService = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name.ToString();
        public static string pubVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

        public static Socket objSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static Dictionary<string, int> pubSendError = new Dictionary<string, int>();

        private static bool IsSocketConnected(Socket s)
        {
            return !((s.Poll(1000, SelectMode.SelectRead) && (s.Available == 0)) || !s.Connected);
        }

        private static void DoWork()
        {
            while (!IsStop)
            {
                IsWork = true;

                bool retValue = IsSocketConnected(objSocket);
                if (!retValue)
                {
                    SocketConnect();
                }
                if (!retValue)
                {
                    WriteEventLog("Error, An error occured, Exception : Socket Connection Refused.", "Error");
                }

                Working();
               

                objSocket.Close();
                IsWork = false;

                Thread.Sleep(int.Parse(Properties.Settings.Default.Interval) * 1000);
            }
        }

        private static void Working()
        {
            WriteEventLog(pubService + " Working Start");

            try
            {
                string curTime = DateTime.Now.ToString("HH:mm");
                if (curTime.Equals("01:30"))
                {
                    //保留3個月
                    RemoveFile(Properties.Settings.Default.ExportPath + "/WebDone/", int.Parse(Properties.Settings.Default.MaxExpHL7Day));
                    RemoveFile(Properties.Settings.Default.ExportPath + "/WebError/", int.Parse(Properties.Settings.Default.MaxExpHL7Day));
                    //保留1個月
                    RemoveFile(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "/Log/", int.Parse(Properties.Settings.Default.MaxExpLogDay));

                    //清除Log
                    EventLog objEventLog = new EventLog();
                    objEventLog.Source = pubService + "Src";
                    objEventLog.Clear();
                }
            }
            catch (Exception ex)
            {
                WriteEventLog("Error, An error occurred, Exception: " + ex.Message, "Error");
            }

            string exportPath = Properties.Settings.Default.ExportPath;
            DirectoryInfo dirInfo = new DirectoryInfo(exportPath);
            FileInfo[] archives = dirInfo.GetFiles("*.HL7", SearchOption.TopDirectoryOnly);

            int iCount = 0;
            foreach (FileInfo hl7File in archives)
            {
                if (IsStop)
                {
                    WriteEventLog("Warning, Terminate the service", "Error");
                    IsWork = false;
                    break;
                }

                iCount++;
                try
                {
                    if (!string.IsNullOrEmpty(hl7File.FullName))
                    {
                        string strHL7Message = File.ReadAllText(hl7File.FullName);
                        ReturnMessage retObject = SenderHL7(strHL7Message);
                        
                        string oStat = "";
                        string oReturnMessage = "";
                        if (retObject.IsSuccess == true)
                        {
                            oStat = "Done";
                            oReturnMessage = "IsSuccess: " + retObject.IsSuccess + ", ACK: " + "\r\n" + retObject.ReceiveMessage;

                            string oWriteMessage = "\r\n" + "\r\n" + DateTime.Now.ToString("yyyy/MM/dd HH:mm") +
                                " FileName: " + hl7File.Name + "\r\n" + "\r\n" + strHL7Message + "\r\n" + oReturnMessage;

                            string newFullName = WriteHL7Message(exportPath, "WebDone", hl7File.Name, hl7File.FullName, oWriteMessage);

                            if (pubSendError.ContainsKey(hl7File.Name))
                            {
                                pubSendError.Remove(hl7File.Name);
                            }
                        }
                        else if ((retObject.ErrorMessage.Equals("Failed, Connection Refused.")) ||
                            (retObject.ErrorMessage.Equals("Failed, Send Error.")) || (retObject.ErrorMessage.Equals("Failed, Send UnACK.")))
                        {
                            oStat = "Resend";
                            oReturnMessage = "IsSuccess : " + retObject.IsSuccess + " , Error : " + retObject.ErrorMessage;

                            if (int.Parse(Properties.Settings.Default.MaxErrorTimes) > 0)
                            {
                                if (pubSendError.ContainsKey(hl7File.Name))
                                {
                                    pubSendError[hl7File.Name] = pubSendError[hl7File.Name] + 1;
                                    if (pubSendError[hl7File.Name] > int.Parse(Properties.Settings.Default.MaxErrorTimes))
                                    {
                                        oStat = "UnDone";
                                        oReturnMessage = "IsSuccess : " + retObject.IsSuccess + " , Error : " + retObject.ErrorMessage;

                                        string newFullName = MoveTCPHL7(exportPath, "UnDone", hl7File.Name, hl7File.FullName);
                                        if (pubSendError.ContainsKey(hl7File.Name))
                                        {
                                            pubSendError.Remove(hl7File.Name);
                                        }
                                    }
                                }
                                else
                                {
                                    pubSendError.Add(hl7File.Name, 1);
                                }
                            }
                        }
                        else if ((retObject.ErrorMessage.Equals("Failed, HL7 is Null.")))
                        {
                            oStat = "Error";
                            oReturnMessage = "IsSuccess : " + retObject.IsSuccess + " , Error : " + retObject.ErrorMessage;

                            string oWriteMessage = "\r\n" + "\r\n" + DateTime.Now.ToString("yyyy/MM/dd HH:mm")
                                + " FileName:" + hl7File.Name + "\r\n" + "\r\n" + strHL7Message + "\r\n" + oReturnMessage;

                            string newFullName = WriteHL7Message(exportPath, "WebDone", hl7File.Name, hl7File.FullName, oWriteMessage);

                            if (pubSendError.ContainsKey(hl7File.Name))
                            {
                                pubSendError.Remove(hl7File.Name);
                            }
                        }
                        else
                        {
                            oStat = "Error";
                            oReturnMessage = "IsSuccess : " + retObject.IsSuccess + " , Error : " + retObject.ErrorMessage;

                            string oWriteMessage = "\r\n" + "\r\n" + DateTime.Now.ToString("yyyy/MM/dd HH:mm") + " FileName:" 
                                + hl7File.Name + "\r\n" + "\r\n" + strHL7Message + "\r\n" + oReturnMessage;

                            string newFullName = WriteHL7Message(exportPath, "WebDone", hl7File.Name, hl7File.FullName, oWriteMessage);

                            if (pubSendError.ContainsKey(hl7File.Name))
                            {
                                pubSendError.Remove(hl7File.Name);
                            }
                        }

                        if (!string.IsNullOrEmpty(retObject.Remarks))
                        {
                            oReturnMessage += " , Remarks : " + retObject.Remarks;
                        }

                        string WriteMessage = "";
                        WriteMessage += "Waiting number : " + (archives.Count() - iCount) + " , Finish : " + iCount + "\r\n";
                        WriteMessage += "FileName : " + hl7File.Name + " , Stat : " + oStat + "\r\n";
                        WriteMessage += oReturnMessage + "\r\n";

                        WriteEventLog(WriteMessage);
                    }
                }
                catch (Exception ex)
                {
                    WriteEventLog("Error, An error occurred whild sending HL7, Exception: " + ex.Message, "Error");
                }
            }

            WriteEventLog(pubService + " Working Stop");
        }

        private static ReturnMessage SenderHL7(string _HL7Message)
        {
            ReturnMessage retObject = new ReturnMessage();
            retObject.IsSuccess = false;
            retObject.ErrorMessage = "Failed, Exception.";

            try
            {
                Stopwatch objWatch = new Stopwatch();
                objWatch.Reset();
                objWatch.Start();

                bool sConnected = IsSocketConnected(objSocket);

                if (!string.IsNullOrEmpty(_HL7Message))
                {
                    try
                    {
                        if (!objSocket.Connected)
                        {
                            bool retValue = SocketConnect();
                            if (!retValue)
                            {
                                retObject.ErrorMessage = "Failed, Connection Refused.";
                                return retObject;
                            }
                        }

                        if (objSocket.Connected)
                        {
                            //Send the Message
                            byte[] aryBuffer = Encoding.UTF8.GetBytes(_HL7Message);
                            objSocket.SendTimeout = 3000;
                            int bytesSent = objSocket.Send(aryBuffer);

                            //Receive the response back
                            if (Properties.Settings.Default.ReturnACK.Equals("1"))
                            {
                                try
                                {
                                    Byte[] aryReceive = new Byte[1024];

                                    objSocket.ReceiveTimeout = 10000;
                                    bytesSent = objSocket.Receive(aryReceive);
                                    Array.Resize(ref aryReceive, bytesSent);

                                    string strReceive = Encoding.Default.GetString(aryReceive).Replace(new string('\0', 255), "");

                                    retObject.IsSuccess = true;
                                    retObject.ReceiveMessage = strReceive;
                                    retObject.ErrorMessage = "";
                                }
                                catch
                                {
                                    retObject.IsSuccess = false;
                                    retObject.ReceiveMessage = "Failed, Send UnACK.";
                                    retObject.ErrorMessage = "Failed, Send UnACK.";
                                }
                            }
                            else
                            {
                                retObject.IsSuccess = true;
                                retObject.ErrorMessage = "";
                            }
                        }
                    }
                    catch
                    {
                        retObject.ErrorMessage = "Failed, Send Error.";
                    }
                }
                else
                {
                    retObject.ErrorMessage = "Failed, HL7 is Null.";
                }

                objWatch.Stop();

                long elapsed = objWatch.ElapsedMilliseconds;
                if (elapsed > (long)5000)
                {
                    retObject.Remarks = "Warning, 執行逾時警告 花費時間 : " + elapsed;
                }
            }
            catch (Exception ex)
            {
                retObject.IsSuccess = false;
                retObject.ErrorMessage = "Failed, " + ex.Message;
            }

            return retObject;
        }

        private static string WriteHL7Message(string _Path, string _Status, string _FileName, string _FullName, string _Message)
        {
            string strReturn = "";

            DateTime _GetTime = DateTime.Now;
            try
            {
                if (!string.IsNullOrEmpty(_FileName))
                {
                    string[] aryInfo = _FileName.Split('_');
                    int iCount = 0;
                    string newFileName = "";
                    foreach (string value in aryInfo)
                    {
                        switch (iCount)
                        {
                            case 0:
                                _GetTime = DateTime.Parse(aryInfo[0].Insert(4, "/").Insert(7, "/").Insert(10, " ").Insert(13, ":").Insert(16, ":"));
                                newFileName = value;
                                break;
                            case 1:
                            case 2:
                                newFileName += "_" + value;
                                break;
                            default:
                                break;
                        }
                        iCount++;
                    }

                    string strPathStatus = _Path + "/" + _Status + "/";
                    Directory.CreateDirectory(strPathStatus);
                    string strPathMon = strPathStatus + "/" + _GetTime.ToString("yyyyMM") + "/";
                    Directory.CreateDirectory(strPathMon);
                    string strPathDay = strPathMon + "/" + _GetTime.ToString("MMdd") + "/";
                    Directory.CreateDirectory(strPathDay);

                    strReturn = strPathDay + newFileName + ".HL7";
                    if (File.Exists(strReturn))
                    {
                        File.AppendAllText(strReturn, _Message);
                    }

                    //移除檔案
                    if (!string.IsNullOrEmpty(_FullName))
                    {
                        if (File.Exists(_FullName))
                        {
                            //移除UnDone檔案
                            File.Delete(_FullName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteEventLog("發生錯誤於 WriteHL7Message , 錯誤說明 : " + ex.Message, "Error");
                return "";
            }
            return strReturn;
        }

        private static bool SocketConnect()
        {
            bool retValue = false;
            try
            {
                TcpClient objTcpClient = new TcpClient();
                IAsyncResult objAsynResult = objTcpClient.BeginConnect(Properties.Settings.Default.ServerIP, int.Parse(Properties.Settings.Default.ServerPort), null, null);
                //只等10秒
                objAsynResult.AsyncWaitHandle.WaitOne(10000, true);

                if (!objAsynResult.IsCompleted)
                {
                    objTcpClient.Close();
                }
                else if (objTcpClient.Connected == true)
                {
                    IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(Properties.Settings.Default.ServerIP), int.Parse(Properties.Settings.Default.ServerPort));
                    objSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    objSocket.Connect(remoteEP);
                    retValue = true;
                }
                else
                {
                    objTcpClient.Close();
                }
                objTcpClient.Close();
            }
            catch
            {
                objSocket.Shutdown(SocketShutdown.Both);
                return SocketConnect();
            }
            return retValue;
        }

        private static bool IsWork { get; set; }
        private static bool IsStop { get; set; }

        private static Thread WorkThread { get; set; }

        public static void Start()
        {
            WriteEventLog(pubService + " Start!");

            WorkThread = new Thread(new ThreadStart(DoWork));
            WorkThread.IsBackground = true;
            IsStop = false;
            WorkThread.Start();
        }

        public static void Stop()
        {
            WriteEventLog(pubService + " Stop!");
            IsStop = true;

            while (IsWork)
            {
                Thread.Sleep(1000);
            }
            WorkThread.Abort();
            WorkThread = null;
        }

        private class ReturnMessage
        {
            public bool IsSuccess { get; set; }
            public string ReceiveMessage { get; set; }
            public string ErrorMessage { get; set; }
            public string Remarks { get; set; }
        }

        public static void WriteEventLog(string _Exception, string _Action = "Log")
        {
            EventLog objEventLog = new EventLog();
            objEventLog.Source = pubService + "Src";

            bool isWrote = false;
            int iCount = 0;

            DateTime _GetTime = DateTime.Now;

            while (isWrote == false && iCount < 3)
            {
                try
                {
                    if (_Action == "Error")
                    {
                        _Exception = "\r\n" + "Exception : " + _Exception + "\r\n";
                    }

                    string sLogPath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + @"\Log\";
                    Directory.CreateDirectory(sLogPath);

                    sLogPath = sLogPath + @"\" + _GetTime.ToString("yyyyMM");
                    Directory.CreateDirectory(sLogPath);

                    String sMessage = _GetTime.ToString("HH:mm:ss") + "\t" + _Exception + "\r\n";
                    File.AppendAllText(Path.Combine(sLogPath, pubService + "_" + "Log" + _GetTime.ToString("yyyyMMdd") + ".txt"), sMessage);

                    if (_Action != "Log")
                        File.AppendAllText(Path.Combine(sLogPath, pubService + "_" + _Action + _GetTime.ToString("yyyyMMdd") + ".txt"), sMessage);

                    objEventLog.WriteEntry(sMessage);

                    isWrote = true;
                }
                catch (Exception ex)
                {
                    Thread.Sleep(500);
                    _Exception = iCount.ToString() + "Write Log Error:" + ex.Message + "\r\n" + _Exception;
                    iCount += 1;
                }
            }
        }

        private static void RemoveFile(string _DirectoryPath, int _Days)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(_DirectoryPath);
            if (Directory.Exists(_DirectoryPath))
            {
                foreach (DirectoryInfo folder in dirInfo.GetDirectories("*", SearchOption.TopDirectoryOnly))
                {
                    //依MMdd目錄查詢
                    foreach (DirectoryInfo subFolder in folder.GetDirectories("*", SearchOption.TopDirectoryOnly))
                    {
                        string overDay = DateTime.Now.AddDays(-_Days).ToString("yyyyMMdd");
                        if (int.Parse(folder.Name.Substring(0, 4) + subFolder.Name) < int.Parse(overDay))
                        {
                            try
                            {
                                //刪除資料夾
                                subFolder.Delete(true);
                            }
                            catch
                            { }
                        }
                    }

                    //查詢所有檔案
                    foreach (FileInfo subFile in folder.GetFiles("*.*", SearchOption.TopDirectoryOnly))
                    {
                        if ((DateTime.Now - subFile.CreationTime).TotalDays > _Days)
                        {
                            try
                            {
                                //刪除檔案
                                subFile.Delete();
                            }
                            catch
                            { }
                        }
                    }

                    //依yyyyMM目錄查詢
                    string overMonth = DateTime.Now.AddDays(-_Days).ToString("yyyyMM");
                    if (int.Parse(folder.Name) < int.Parse(overMonth))
                    {
                        try
                        {
                            //刪除資料夾
                            folder.Delete(true);
                        }
                        catch
                        { }
                    }
                }
            }
        }

        private static string MoveTCPHL7(string path, string status, string fileName, string fullName)
        {
            string retFullName = "";

            try
            {
                DateTime dtNow = DateTime.Now;

                string sPathStatus = path + @"\" + status + @"\";
                Directory.CreateDirectory(sPathStatus);

                string sPathMon = sPathStatus + dtNow.ToString("yyyyMM") + @"\";
                Directory.CreateDirectory(sPathMon);

                string sPathDay = sPathMon + dtNow.ToString("MMdd") + @"\";
                Directory.CreateDirectory(sPathDay);

                retFullName = Path.Combine(sPathDay, fileName);

                //檢查檔案是否重覆,是則移除
                if (File.Exists(retFullName))
                {
                    File.Delete(retFullName);
                }

                File.Move(fullName, retFullName);

                //檢查檔案是否移動,否則移除
                if (!string.IsNullOrEmpty(fullName))
                {
                    //移除UnDone檔案
                    if (File.Exists(fullName))
                    {
                        File.Delete(fullName);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteEventLog("Error, File.Move Failed , Exception : " + ex.Message, "Error");
            }

            return fullName;
        }
    }
}
