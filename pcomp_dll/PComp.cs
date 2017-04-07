using System;
using System.Text;
using System.IO;
using System.Net;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pcomp_dll
{
    class PComp
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main(String[] args)
        {
            UserFile file1 = new UserFile("파일1");
            UserFile file2 = new UserFile("파일2");

            try
            {
                // 입력 매개변수 2개 여부 Check
                if (args.Length != 2)
                {
                    Console.WriteLine("2개의 매개변수만 입력이 가능합니다.(현재 {0}개)", args.Length);
                    Console.WriteLine("예시) pcomp.exe file1 file2 <ENTER>");
                    Console.WriteLine("※ 파일이름에 띄어쓰기가 있을 경우, 별도의 매개변수로 인식할 수 있습니다.");
                    Console.WriteLine("   파일이름에서 띄어쓰기를 제거해주세요.");
                    return;
                }
                else
                {
                    if (string.IsNullOrEmpty(args[0]))
                    {
                        Console.WriteLine("1번째 매개변수를 확인하세요.");
                        return;
                    }
                    if (string.IsNullOrEmpty(args[1]))
                    {
                        Console.WriteLine("2번째 매개변수를 확인하세요.");
                        return;
                    }
                }

                // 경로 및 파일명 셋팅
                if (false == file1.SetData(args[0])) { return; }
                if (false == file2.SetData(args[1])) { return; }

                // 총 라인 수, 비교대상 라인 수 Count
                if (false == file1.CountTotalLine()) { return; }
                if (false == file2.CountTotalLine()) { return; }

                if (file1.FileCompareLine >= file2.FileCompareLine)
                {
                    // 파일1을 기준으로 파일2와 비교
                    if (false == file1.CompareFile(file2)) { return; }
                }
                else
                {
                    // 파일2를 기준으로 파일1와 비교
                    if (false == file2.CompareFile(file1)) { return; }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("예외 발생:{0}", e.Message);
                return;
            }
        }

    }



    /// <summary>
    /// UserFile Class
    /// 파일경로, 파일명, 파일전체경로 정보 및
    /// 두 파일을 비교 후 결과를 txt파일로 저장하는 method를 가진 Class.
    /// </summary>
    /// <remarks>
    /// 2017.03.30. 장민수 최초작성.
    /// </remarks>
    public class UserFile
    {
        public long FileTotalLine; // 파일의 총 라인수
        public long FileCompareLine; // 파일의 비교대상 라인수

        public string Name { get; set; } // 파일명
        public string FilePath { get; set; } //파일경로
        public string FileName { get; set; } //파일명.확장자
        public string FileFullName { get; set; } //파일경로\파일명.확장자
        public string CompareResult { get; set; } // 비교결과

        /// <summary>
        /// 출력파일 이름을 셋팅하는 생성자(name+CompareResult.txt 로 출력됨).
        /// </summary>
        public UserFile(string strName)
        {
            this.Name = strName;
        }

        /// <summary>
        /// Data 초기화
        /// </summary>
        /// <remarks>
        /// 2017.03.30. 장민수 최초작성.
        /// </remarks>
        public void ClearData()
        {
            this.Name = null;
            this.FilePath = null;
            this.FileName = null;
            this.FileFullName = null;
            this.CompareResult = null;
            return;
        }


        /// <summary>
        /// 경로 및 파일명 셋팅
        /// <param name="strFileFullPath">파일경로\파일명.확장자</param>
        /// </summary>
        /// <remarks>
        /// 2017.03.30. 장민수 최초작성.
        /// </remarks>
        public bool SetData(string strFileFullPath)
        {
            try
            {
                // 파일경로\파일명.확장자
                this.FileFullName = strFileFullPath;

                // '\' 문구로 경로 입력여부 Check
                if (strFileFullPath.LastIndexOf(@"\") < 0)
                {
                    Console.WriteLine("경로를 정확히 입력하세요.");
                    return false;
                }
                else
                {
                    // 파일경로
                    this.FilePath = strFileFullPath.Substring(0, strFileFullPath.LastIndexOf(@"\"));

                    // 파일명.확장자
                    this.FileName = strFileFullPath.Substring(strFileFullPath.LastIndexOf(@"\") + 1, strFileFullPath.Length - strFileFullPath.LastIndexOf(@"\") - 1);

                    // 파일명
                    if (FileName.LastIndexOf(".") >= 0)
                    {
                        this.Name = FileName.Substring(0, FileName.LastIndexOf("."));
                    }
                    else
                    {
                        Console.WriteLine("파일명을 정확히 입력하세요.(확장자 없음)");
                        return false;
                    }
                }

                // Console.WriteLine("{0} SetData() => [{1}][{2}][{3}]", GetName(), GetFileFullName(), GetFilePath(), GetFileName());

                // 파일경로 Check
                if (false == CheckReadFile()) { return false; }
            }
            catch (Exception e)
            {
                Console.WriteLine("예외 발생:{0}", e.Message);
                return false;
            }

            return true;
        }


        /// <summary>
        /// 디렉토리 및 파일 Check(null, 경로존재 여부 등)
        /// </summary>
        /// <remarks>
        /// 2017.03.30. 장민수 최초작성.
        /// </remarks>
        /// <returns>true: 정상, false: 비정상.</returns>
        public bool CheckReadFile()
        {
            try
            {
                // 파일명 null Check
                if (string.IsNullOrEmpty(this.Name))
                {
                    Console.WriteLine("파일이 제대로 선택되지 않았습니다.");
                    return false;
                }

                // 파일경로 null Check
                if (string.IsNullOrEmpty(this.FilePath))
                {
                    Console.WriteLine("[{0}] 디렉토리가 입력되지 않았습니다.", this.Name);
                    return false;
                }

                // 파일명.확장자 null Check
                if (string.IsNullOrEmpty(this.FileName))
                {
                    Console.WriteLine("[{0}] 파일명이 입력되지 않았습니다.", this.Name);
                    return false;
                }

                // 해당 디렉토리가 존재여부 Check
                DirectoryInfo di = new DirectoryInfo(this.FilePath);
                if (!di.Exists)
                {
                    Console.WriteLine("[{0}] 해당 디렉토리가 존재하지 않습니다.({1})", this.Name, this.FilePath);
                    return false;
                }

                // 해당 디렉토리에 파일 존재여부 Check
                FileInfo fi = new FileInfo(this.FileFullName);
                if (!fi.Exists)
                {
                    Console.WriteLine("[{0}] 해당 디렉토리에 파일이 존재하지 않습니다.({1})", this.Name, this.FileFullName);
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("예외 발생:{0}", e.Message);
                return false;
            }

            return true;
        }


        /// <summary>
        /// 파일의 라인 개수 Count
        /// </summary>
        /// <remarks>
        /// 2017.04.03. 장민수 최초작성.
        /// </remarks>
        /// <returns>true: 정상, false: 비정상.</returns>
        public bool CountTotalLine()
        {
            try
            {
                // 파일의 경로, 파일유무 Check
                if (false == this.CheckReadFile()) { return false; }

                // 파일을 읽어 Buffer에 저장
                // 한글 인코딩 문제로 "euc-kr"을 Default 지정.
                StreamReader srFile;
                srFile = new StreamReader(this.FileFullName, Encoding.GetEncoding("euc-kr"));

                // Buffer null Check
                if (srFile == null)
                {
                    Console.WriteLine("[{0}]은(는) 비어있는 파일입니다.\n", this.FileName);
                    return false;
                }


                //////////////////////////////// 전체 라인 수 Count ////////////////////////////////
                // 라인 수 초기화
                FileTotalLine = 0;

                // 파일 처음부터 끝까지 Read
                string getFile = srFile.ReadToEnd();

                // 줄바꿈부호(\r)가 있으면 Count
                foreach (char c in getFile)
                {
                    FileTotalLine = c.Equals('\r') ? FileTotalLine + 1 : FileTotalLine;
                }
                ////////////////////////////////////////////////////////////////////////////////////

                //////////////////////////////// 비교대상 라인 수 Count ////////////////////////////////
                // 라인 수 초기화
                FileCompareLine = 0;

                // 포인터 시작점으로 옮기기
                srFile.BaseStream.Position = 0;

                // Read File Line
                do
                {
                    if (srFile.Peek() > -1)
                    {
                        FileCompareLine = srFile.ReadLine().Trim().Equals("") ? FileCompareLine : FileCompareLine + 1;
                    }
                } while (!srFile.EndOfStream); // EOF까지 반복
                ////////////////////////////////////////////////////////////////////////////////////////

                srFile.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("예외 발생:{0}", e.Message);
                return false;
            }

            return true;
        }


        /// <summary>
        /// 현재 클래스의 파일(파일1)과 인자로 받은 비교대상 파일(파일2)을 버퍼에 저장하여 비교 후
        /// 차이나는 부분을 파일로 저장한다.(파일명, 라인, 내용)
        /// <param name="file">비교대상 UserFile Class</param>
        /// </summary>
        /// <remarks>
        /// 2017.03.30. 장민수 최초작성.
        /// </remarks>
        /// <returns>true: 정상, false: 비정상.</returns>
        public bool CompareFile(UserFile file)
        {
            try
            {
                // 파일1의 경로, 파일유무 Check
                if (false == this.CheckReadFile()) { return false; }
                // 파일2의 경로, 파일유무 Check
                if (false == file.CheckReadFile()) { return false; }

                // 파일을 읽어 Buffer에 저장
                // 한글 인코딩 문제로 "euc-kr"을 Default 지정.
                StreamReader srFile1;
                StreamReader srFile2;
                srFile1 = new StreamReader(this.FileFullName, Encoding.GetEncoding("euc-kr"));
                srFile2 = new StreamReader(file.FileFullName, Encoding.GetEncoding("euc-kr"));

                String strFile1Line = "";
                String strFile2Line = "";
                long lFile1LineNum = 0;
                long lFile2LineNum = 0;
                int iPeek1 = 0;
                int iPeek2 = 0;
                CompareResult = "";

                // 파일1 기준으로 EOF까지 비교
                do
                {
                    ////////////////////////////////////////////////////////////////////////////////
                    // Read File1 Line
                    ////////////////////////////////////////////////////////////////////////////////
                    do
                    {
                        iPeek1 = srFile1.Peek();
                        if (iPeek1 > -1)
                        {
                            lFile1LineNum++;
                            strFile1Line = srFile1.ReadLine();
                        }
                    } while ((strFile1Line == "") && (!srFile1.EndOfStream)); // EOF가 아니고, 비어있으면 무시

                    ////////////////////////////////////////////////////////////////////////////////
                    // Read File2 Line
                    ////////////////////////////////////////////////////////////////////////////////
                    do
                    {
                        iPeek2 = srFile2.Peek();
                        if (iPeek2 > -1)
                        {
                            lFile2LineNum++;
                            strFile2Line = srFile2.ReadLine();
                        }
                        else
                        {
                            // 파일2를 끝까지 다 읽었을 경우 '#####<EMPTY>#####' 문구 출력.
                            lFile2LineNum = 0;
                            strFile2Line = "#####<EMPTY>#####";
                        }
                    } while ((strFile2Line == "") && (!srFile2.EndOfStream)); // EOF가 아니고, 비어있으면 무시

                    // 파일2를 끝까지 다 읽었는데, 내용이 비어있을 경우 '#####<EMPTY>#####' 문구 출력.
                    if ((strFile2Line == "") && (srFile2.EndOfStream))
                    {
                        lFile2LineNum = 0;
                        strFile2Line = "#####<EMPTY>#####";
                    }
                    ////////////////////////////////////////////////////////////////////////////////

                    // 파일1의 내용이 비어있지 않고, 파일2의 내용과 다르면 CompareResult에 파일1의 내용을 기록
                    ////////////////////////////////////////////////////////////////////////////////
                    if ((strFile1Line.Trim() != "") && (strFile1Line.Trim() != strFile2Line.Trim()))
                    {
                        CompareResult += "-----------------------------------------------------------------------------------\r\n";
                        CompareResult += "[" + this.FileName + " (" + lFile1LineNum + ")]\t" + strFile1Line + "\r\n";
                        CompareResult += "[" + file.FileName + " (" + lFile2LineNum + ")]\t" + strFile2Line + "\r\n";
                    }
                    ////////////////////////////////////////////////////////////////////////////////
                } while (!srFile1.EndOfStream); // EOF까지

                // StreamReader Close
                srFile1.Close();
                srFile2.Close();


                if (CompareResult.Length > 0)
                {
                    Console.WriteLine("Files are not equal.");

                    CompareResult += "-----------------------------------------------------------------------------------\r\n";
                    Console.WriteLine("{0}", CompareResult);

                    // 파일 쓰기
                    if (false == WriteFile(CompareResult)) { return false; }
                }
                else
                {
                    Console.WriteLine("Files are equal.");
                }
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.WriteLine("예외 발생:{0}", e.Message);
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine("예외 발생:{0}", e.Message);
                return false;
            }

            return true;
        }


        /// <summary>
        /// 매개변수로 전달받은 데이터를 파일로 저장한다.
        /// 파일저장경로 : 현재 비교대상이었던 파일의 경로(m_strFilePath)
        /// 파일명       : 파일이름+CompareResult.txt
        /// <param name="strData">두 파일의 비교 결과가 담긴 데이터</param>
        /// </summary>
        /// <remarks>
        /// 2017.03.30. 장민수 최초작성.
        /// </remarks>
        /// <returns>true: 정상, false: 비정상.</returns>
        public bool WriteFile(string strData)
        {
            try
            {
                // 현재 클래스에 저장되어있는 경로(m_strFilePath)에 파일이름+CompareResult.txt로 만듦
                // 한글 인코딩 문제로 "euc-kr"을 Default 지정.
                StreamWriter swFile = new StreamWriter(this.FilePath + @"\" + this.Name + "_CompareResult.txt", false, Encoding.GetEncoding("euc-kr"), strData.Length);
                swFile.Write(strData);
                swFile.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine("예외 발생:{0}", e.Message);
                return false;
            }
            return true;
        }

    }
}
