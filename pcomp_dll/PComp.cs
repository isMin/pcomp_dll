using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace pcomp
{
    /// <summary>
    /// 파일의 정보를 갖고, Create, Init, Read, Write, Check 기능을 하는 클래스.
    /// </summary>
    /// <remarks>
    /// created : 2017.04.10.
    /// writer  : 장민수
    /// </remarks>
    class PFile
    {
        // 파일의 정보
        private FileInfo fi;
        private StreamReader srFile;
        private StreamWriter swFile;


        /// <summary>
        /// 파일정보 객체를 생성하는 생성자.
        /// </summary>
        /// <remarks>
        /// created : 2017.04.10.
        /// writer  : 장민수
        /// </remarks>
        /// <param name="fullName">전체경로</param>
        public PFile(string fullName)
        {
            // 객체 할당.
            fi = new FileInfo(fullName);
            //Console.WriteLine("fi.FullName: ({0})\nfi.Directory: ({1})\nfi.Name: ({2})\nfi.Extension: ({3})\nfi.DirectoryName: ({4})\nfi.Length: ({5})\nfi.ToString(): ({6})\n", fi.FullName, fi.Directory, fi.Name, fi.Extension, fi.DirectoryName, fi.Length, fi.ToString());
        }

        /// <summary>
        /// 파일의 경로(디렉토리) Get
        /// </summary>
        /// <remarks>
        /// created : 2017.04.10.
        /// writer  : 장민수
        /// </remarks>
        /// <returns>파일의 경로(디렉토리)</returns>
        public string GetDirectoryName()
        {
            return fi.DirectoryName;
        }

        /// <summary>
        /// 파일의 내용을 담고있는 StreamReader.
        /// </summary>
        /// <remarks>
        /// created : 2017.04.10.
        /// writer  : 장민수
        /// </remarks>
        /// <returns>할당된 StreamReader 객체</returns>
        public StreamReader GetStreamReader()
        {
            return srFile;
        }

        /// <summary>
        /// 파일의 내용을 StreamReader에 저장.
        /// </summary>
        /// <remarks>
        /// created : 2017.04.10.
        /// writer  : 장민수
        /// </remarks>
        /// <returns>true: 정상, false: 비정상.</returns>
        public bool SetStreamReader()
        {
            try
            {
                // 파일을 읽어 Buffer에 저장함(한글 인코딩 문제로 "euc-kr"을 Default 지정).
                srFile = new StreamReader(fi.FullName, Encoding.GetEncoding("euc-kr"));

                // 파일의 내용이 비어있는지 체크.
                if (null == srFile)
                {
                    Console.WriteLine("[{0}] 파일은 비어있습니다.", fi.FullName);
                    srFile.Close();
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
        /// 파일 생성 및 초기화
        /// </summary>
        /// <remarks>
        /// created : 2017.04.10.
        /// writer  : 장민수
        /// </remarks>
        /// <returns>true: 정상, false: 비정상.</returns>
        public bool InitTextFile()
        {
            try
            {
                FileStream fsFile = fi.OpenWrite();
                fsFile.SetLength(0);
                fsFile.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("예외 발생:{0}", e.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 해당 파일의 존재여부 및 파일이 비어있는지 여부 체크.
        /// </summary>
        /// <remarks>
        /// created : 2017.04.10.
        /// writer  : 장민수
        /// </remarks>
        /// <returns>true: 정상, false: 비정상.</returns>
        public bool Check()
        {
            try
            {
                // 디렉토리 존재여부 체크.
                if (!fi.Directory.Exists)
                {
                    Console.WriteLine("해당 디렉토리가 존재하지 않습니다.({0})", fi.FullName);
                    return false;
                }
                // 파일 존재여부 체크.
                if (!fi.Exists)
                {
                    Console.WriteLine("해당 파일이 존재하지 않습니다.({0})", fi.FullName);
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
        /// 콘솔에 해당 라인과 내용을 포맷팅하여 출력.
        /// </summary>
        /// <param name="iFile1Line">파일1 라인번호</param>
        /// <param name="strFile1Line">파일1 라인내용</param>
        /// <param name="iFile2Line">파일2 라인번호</param>
        /// <param name="strFile2Line">파일2 라인내용</param>
        public void Show(int iFile1Line, string strFile1Line, int iFile2Line, string strFile2Line)
        {
            Console.Write("-----------------------------------------------------------------------------------\r\n");
            Console.Write("[File1 ({0})]\t{1}\r\n", iFile1Line, strFile1Line);
            Console.Write("[File2 ({0})]\t{1}\r\n", iFile2Line, strFile2Line);
        }

        /// <summary>
        /// 출력파일에 라인과 내용을 포맷팅하여 출력.
        /// </summary>
        /// <param name="iFile1Line">파일1 라인번호</param>
        /// <param name="strFile1Line">파일1 라인내용</param>
        /// <param name="iFile2Line">파일2 라인번호</param>
        /// <param name="strFile2Line">파일2 라인내용</param>
        /// <returns>true: 정상, false: 비정상.</returns>
        public bool WriteTextFile(int iFile1Line, string strFile1Line, int iFile2Line, string strFile2Line)
        {
            try
            {
                // 기존 파일의 내용에 텍스트를 추가.
                swFile = fi.AppendText();

                swFile.WriteLine("-----------------------------------------------------------------------------------");
                swFile.WriteLine("[File1 ({0})]\t{1}", iFile1Line, strFile1Line);
                swFile.WriteLine("[File2 ({0})]\t{1}", iFile2Line, strFile2Line);
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


    /// <summary>
    /// 두 파일을 읽어 비교한 후 콘솔/텍스트파일로 결과를 출력하는 클래스.
    /// </summary>
    /// <remarks>
    /// created : 2017.04.10.
    /// writer  : 장민수
    /// </remarks>    
    class PCompare
    {
        int iFile1Line; // 파일1의 라인수.
        int iFile2Line; // 파일2의 라인수.

        /// <summary>
        /// 두 파일을 읽어 비교한 후 콘솔/텍스트파일로 결과를 출력.
        /// </summary>
        /// <param name="File1FullName">파일1 전체경로</param>
        /// <param name="File2FullName">파일2 전체경로</param>
        /// <remarks>
        /// created : 2017.04.10.
        /// writer  : 장민수
        /// </remarks>
        /// <returns>true: 정상, false: 비정상.</returns>
        public bool Compare(string File1FullName, string File2FullName)
        {
            try
            {
                // 두 파일의 객체 생성.
                PFile file1 = new PFile(File1FullName);
                PFile file2 = new PFile(File2FullName);

                // 파일 체크.
                if (false == file1.Check()) { return false; }
                if (false == file2.Check()) { return false; }

                // 파일의 내용을 StreamReader로 Set
                if (false == file1.SetStreamReader()) { return false; }
                if (false == file2.SetStreamReader()) { return false; }


                bool bFirst = true; // 최초 1회만 수행. 파일 생성 및 초기화.
                string strFile1Line = ""; // 파일1의 라인내용.
                string strFile2Line = ""; // 파일2의 라인내용.
                iFile1Line = 0; // 파일1의 라인수.
                iFile2Line = 0; // 파일2의 라인수.

                // 파일1, 파일2의 끝에 도달할 때까지 반복하여 수행.
                while ((!file1.GetStreamReader().EndOfStream) || (!file2.GetStreamReader().EndOfStream)) // EOF까지
                {
                    // Read 파일1 Line
                    strFile1Line = ReadLine(file1, 1);
                    if (null == strFile1Line) { return false; }

                    // Read File2 Line
                    strFile2Line = ReadLine(file2, 2);
                    if (null == strFile2Line) { return false; }

                    ////////////////////////////////////////////////////////////////////////////////
                    // 두 라인을 비교하여 다를경우, 콘솔 및 텍스트파일에 출력(공백라인 무시).
                    ////////////////////////////////////////////////////////////////////////////////
                    if ("" != strFile1Line && "" != strFile2Line
                        && strFile1Line != strFile2Line)
                    {
                        // 비교결과를 기록할 파일 객체 생성.
                        PFile resultFile = new PFile(file1.GetDirectoryName() + @"\Files_CompareResult.txt");

                        // 최초 1회 수행시 텍스트파일 초기화 및 체크.
                        if (true == bFirst)
                        {
                            // 텍스트파일 초기화.
                            if (false == resultFile.InitTextFile()) { return false; }

                            // 텍스트파일 체크.
                            if (false == resultFile.Check()) { return false; }

                            // 첫 수행시에만 동작하도록 false로 수정.
                            bFirst = false;
                        }

                        // 비교결과를 콘솔에 출력.
                        resultFile.Show(iFile1Line, strFile1Line, iFile2Line, strFile2Line);

                        // 비교결과를 파일에 출력.
                        resultFile.WriteTextFile(iFile1Line, strFile1Line, iFile2Line, strFile2Line);
                    }
                    ////////////////////////////////////////////////////////////////////////////////
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
        /// 파일의 한 줄을 읽어서 String 타입으로 반환함(공백라인 무시).
        /// </summary>
        /// <param name="file">파일정보가 담긴 PFile 객체</param>
        /// <param name="selectFile">파일1: 1, 파일2: 2</param>
        /// <returns>파일에서 읽어들인 라인의 데이터 내용(String)</returns>
        public string ReadLine(PFile file, int selectFile)
        {
            string strFileLine = "";
            try
            {
                if (selectFile != 1 && selectFile != 2)
                {
                    throw (new Exception("ReadLine 함수 호출 시, selectFile 인자값은 1,2 중에 선택하세요."));
                }

                do
                {
                    // 다음 문자로 Peek
                    file.GetStreamReader().Peek();

                    // 파일의 끝이 아니면, 해당 라인을 읽어 string변수에 저장.
                    if (!file.GetStreamReader().EndOfStream)
                    {
                        // 파일1 라인수 카운트.
                        if (selectFile == 1) { iFile1Line++; }
                        // 파일2 라인수 카운트.
                        else if (selectFile == 2) { iFile2Line++; }

                        // 한 라인을 읽어 string변수에 저장.
                        strFileLine = file.GetStreamReader().ReadLine();

                        // 공백라인 무시.
                        if ("" != strFileLine) { break; }

                        // 마지막라인이 공백라인이면 문구 셋팅 후 break;
                        if (("" == strFileLine) && file.GetStreamReader().EndOfStream)
                        {
                            // 파일1 라인수 0으로 초기화.
                            if (selectFile == 1) { iFile1Line = 0; }
                            // 파일2 라인수 0으로 초기화.
                            else if (selectFile == 2) { iFile2Line = 0; }

                            // '#####<EMPTY>#####' 문구로 셋팅
                            strFileLine = "#####<EMPTY>#####";
                            break;
                        }
                    }
                    else
                    {
                        // 파일1 라인수 0으로 초기화.
                        if (selectFile == 1) { iFile1Line = 0; }
                        // 파일2 라인수 0으로 초기화.
                        else if (selectFile == 2) { iFile2Line = 0; }

                        // '#####<EMPTY>#####' 문구로 셋팅
                        strFileLine = "#####<EMPTY>#####";
                    }
                } while (!file.GetStreamReader().EndOfStream); // 파일1의 끝까지 반복하여 수행.
            }
            catch (Exception e)
            {
                Console.WriteLine("예외 발생:{0}", e.Message);
                return null;
            }
            return strFileLine;
        }

    }
}
