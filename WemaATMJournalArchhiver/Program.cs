using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;
using WemaATMJournalArchhiver.DATA;
using System.Security.Principal;
using System.Security.AccessControl;

namespace WemaATMJournalArchhiver
{
    class Program
    {
        int noofdirectories = 0;
        List<string> Initialdirectorynames = null;
        List<string> subdirectorynames = null;
        List<string> innerdirectorynames = null;    
        string newdirectory = null;
        string newdirectoryname = null;       
        string []filenames = null;
        string lastfourdigitsoffilename = null;      
        string fiilename = null;
        string fiilenamewithextension = null;
        string newlocation = null;
        string fullpath = null;
        string newfile = null;
        List<atm_ip_address> ipadd = null;     
        string branchfolder = null;
        string newdirectorypath = null;
        string month = null;
        string textmonth = null;
        ///string actualmonthname = null;
        string branch = null;
        string iipaddress = null;

        

        static void Main(string[] args)
        {
            //Console Application to archive files stored in the ATM journal folder created everyday 
            //and used by E-business. Files here are copied to a different directory for use by the ATM Journal application.           
            Console.WriteLine("Starting Console Application. Preparing to copy files...");          
            new Program().ArchiveFiles();            
        }


        private void ArchiveFiles()
        {
            //Create EF object
            ATMArchiverEntities ate = new ATMArchiverEntities();

            try
            {
                //Retrieve all the ipaddresses from the database and store in a list object. Iterate through to create the
                //for the branch and store the file in it.
                //Return all the directories inside the directory in the AppSettings part of the Web.Config file
                //Get total number of folders in the Parent directory to keep track.
                ipadd = new List<atm_ip_address>();

                var ipaddress = from ip in ate.atm_ip_address select ip;

                if (ipaddress == null)
                {
                    Console.WriteLine("No ip address could be found in the database.");
                }
                else
                {
                    foreach (atm_ip_address ip1 in ipaddress)
                    {
                        //Retrieve the ipaddress and add it to the list object.
                        ipadd.Add(ip1);
                    }
                }

                string a = @"\\";
                //Replace PreviousFolders with Journal once application is ready for shipping.
                string b = @"\\Journal";

                foreach (atm_ip_address dir in ipadd)
                {                   
                    noofdirectories = Directory.GetDirectories(a+dir.ipaddress+b).Length;
                    if (noofdirectories != 0)
                    {
                        Initialdirectorynames = new List<string>();
                        foreach (string folder in Directory.GetDirectories(a+dir.ipaddress+b))
                        {
                            Initialdirectorynames.Add(Path.GetFileName(folder));
                        }
                        Console.WriteLine("Number of directories found in parent directory: " + noofdirectories);
                        //Retrieve the folder name in the directorynames list object.
                        subdirectorynames = new List<string>();
                        innerdirectorynames = new List<string>();

                        foreach (string subdirectory in Initialdirectorynames)
                        {
                           foreach (string newpath in Directory.GetDirectories(a+dir.ipaddress+b + "\\"))
                            {
                                //Add new folder to new list object
                                filenames = Directory.GetFiles(newpath);
                                foreach (string f in filenames)
                                {
                                    fiilename = Path.GetFileNameWithoutExtension(f);
                                    fiilenamewithextension = Path.GetFileName(f);
                                    lastfourdigitsoffilename = fiilename.Substring(4, 4);
                                    month = fiilename.Substring(2, 2);
                                    fullpath = newpath + "\\" + fiilenamewithextension;
                                   
                                    //Pull the new path from the database instead.
                                    //Move to live server.
                                   // newdirectorypath = @"\\172.27.4.227\\ATMJournal\\";
                                  //  newdirectorypath = @"\\172.27.4.227\c$\\inetpub\wwwroot\ATMJournal\App_Upload\";
                                    //newdirectorypath = @"\\WEMA-HQ-BA-TEST\App_Upload\";

                                    newdirectorypath =  @"\\WEMA-HQ-BA-TEST\App_Upload\";

                                    //Moving files to new directory.
                                    //Check for a folder in the C:/ATMJournal directory and if it doesn't exist, create it.
                                    branch = dir.branch;
                                    branchfolder = dir.branch+"\\";
                                    //Get the actual month
                                   // actualmonthname = GetMonthName(month);
                                    
                                    newdirectory = newdirectorypath+branchfolder+"\\";
                                    newdirectoryname = lastfourdigitsoffilename +"\\";
                                    newlocation = newdirectory + newdirectoryname +"\\";
                                    newfile = newlocation + fiilenamewithextension;

                                    if (Directory.Exists(newlocation))
                                    {
                                        //Move file to the directory
                                        if (File.Exists(newfile))
                                        {
                                            Console.WriteLine("File " + fullpath + " already exists in the directory.");
                                            //log information
                                            new Program().LogFile("File " + fullpath + " already exists in the directory.");

                                           // deleteCheck(newfile);
                                        }
                                        else
                                        {
                                            File.SetAttributes(fullpath, FileAttributes.ReadOnly);
                                           
                                            File.Copy(fullpath, newfile);

                                           // deleteCheck(fullpath);

                                            Console.WriteLine("File " + fullpath + " successfully moved to the directory.");
                                            //log information                                     
                                             new Program().LogFile("File " + fullpath + " successfully moved to the directory.");
                                            
                                            //Save the path to the database
                                            new_atm_log_path newatmpath = new new_atm_log_path();
                                            newatmpath.path_id = Guid.NewGuid();
                                            newatmpath.branch_code = branch;
                                            newatmpath.year = lastfourdigitsoffilename;
                                            newatmpath.filename = fiilenamewithextension;
                                            newatmpath.new_file_path = newfile;

                                            //Add Ip address and terminal id
                                            newatmpath.ipaddress = dir.ipaddress;
                                            newatmpath.terminalid = dir.terminalid;
                                            newatmpath.datecreated = DateTime.Now;

                                            ate.new_atm_log_path.Add(newatmpath);
                                            ate.SaveChanges();
                                  }
                                        
                                        
                                    }
                                    else
                                    { 
                                        //Create the directory and move the file
                                        Directory.CreateDirectory(newlocation);
                                        
                                        if (File.Exists(newfile))
                                        {
                                            Console.WriteLine("File " + fullpath + " already exists in the directory.");
                                            //log information
                                            new Program().LogFile("File " + fullpath + " already exists in the directory.");
                                        }
                                        else
                                        {
                                           
                                            File.SetAttributes(fullpath, FileAttributes.ReadOnly);
                                            File.Copy(fullpath, newfile);

                                           // deleteCheck(newfile);
                                            Console.WriteLine("File " + fullpath + " successfully moved to the directory.");
                                            //log information
                                            new Program().LogFile("File " + fullpath + " already exists in the directory.");


                                            //Save the path to the database
                                            new_atm_log_path newatmpath = new new_atm_log_path();
                                            newatmpath.path_id = Guid.NewGuid();
                                            newatmpath.branch_code = branch;
                                            newatmpath.year = lastfourdigitsoffilename;
                                            newatmpath.filename = fiilenamewithextension;
                                            newatmpath.new_file_path = newfile;
                                            //Add ipaddress and terminal id
                                            newatmpath.ipaddress = dir.ipaddress;
                                            newatmpath.terminalid = dir.terminalid;
                                            newatmpath.datecreated = DateTime.Now;

                                            ate.new_atm_log_path.Add(newatmpath);
                                            ate.SaveChanges();
                                           
                                        }
                                    }
                                }
                            }
                        }
                    }

                    else
                    {
                        Console.WriteLine("No folders from Branches or Off Sites exist in this directory.");
                        new Program().LogFile("No folders from Branches or Off Sites exist in this directory.");
                    }
                }
            }
            catch (Exception e)
            {
                //Log exception errors in text file on the server and in addition, in the SQL Server database


                runtime_errors rt = new runtime_errors();
                rt.id = Guid.NewGuid();
                rt.message = e.ToString();
                rt.date_created = DateTime.Now;
                ate.runtime_errors.Add(rt);
                ate.SaveChanges();

                //Log exception to file               
                  new Program().LogFile(e.ToString());

            }

        }

        public void LogFile(string message)
        {
            StreamWriter log;

            if (!File.Exists("C:/ATMJournallogs/logfile.txt"))
            {
                log = new StreamWriter("C:/ATMJournallogs/logfile.txt");
            }
            else
            {
                log = File.AppendText("C:/ATMJournallogs/logfile.txt");
            }

            // Write to the file:

            log.WriteLine("Activity date/time:" + DateTime.Now);

            log.WriteLine("Message:" + message);

            // Close the stream:

            log.Close();

        }

        public string GetMonthName(string month)
        {
            if (month == "01")
            {
                textmonth = "January";
            }
            else if (month == "02")
            {
                textmonth = "February";
            }
            else if (month == "03")
            {
                textmonth = "March";
            }
            else if (month == "04")
            {
                textmonth = "April";
            }
            else if (month == "05")
            {
                textmonth = "May";
            }
            else if (month == "06")
            {
                textmonth = "June";
            }
            else if (month == "07")
            {
                textmonth = "July";
            }
            else if (month == "08")
            {
                textmonth = "August";
            }
            else if (month == "09")
            {
                textmonth = "September";
            }
            else if (month == "10")
            {
                textmonth = "October";
            }
            else if (month == "11")
            {
                textmonth = "November";
            }
            else if (month == "12")
            {
                textmonth = "December";
            }

            return textmonth;
        }

        private void deleteCheck(string path)
        {


            FileInfo fileinfo = new FileInfo(path);
            if (fileinfo.IsReadOnly)
            {

                Console.WriteLine("The file cannot be deleted.");
            }


        }
    }
    }




