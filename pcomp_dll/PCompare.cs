using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using pfile;

namespace pcompare
{
    /// <summary>
    /// 두 파일을 읽어 비교한 후 콘솔/텍스트파일로 결과를 출력하는 클래스.
    /// </summary>
    /// <remarks>
    /// created : 2017.04.10.
    /// writer  : 장민수
    /// </remarks>    
    public static class PCompare
    {
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
        public static bool Compare(string File1FullName, string File2FullName)
        {
            try
            {
                // 두 파일의 객체 생성.
                PFile file1 = new PFile(File1FullName);
                PFile file2 = new PFile(File2FullName);

                // 파일 체크.
                if (false == file1.Check()) { return false; }
                if (false == file2.Check()) { return false; }

                // StreamReader 객체 할당
                if (false == file1.SetStreamReader()) { return false; }
                if (false == file2.SetStreamReader()) { return false; }


                // 비교결과를 기록할 파일 객체 생성.
                PFile resultFile = new PFile(file1.DirectoryName + @"\Files_CompareResult.txt");

                bool bInit = false; // 초기화 여부(최초 1회만 수행. 파일 생성 및 초기화).
                file1.LineNum = 0; // 파일1의 현재 라인.
                file2.LineNum = 0; // 파일2의 현재 라인.

                // 파일1, 파일2의 끝에 도달할 때까지 반복하여 수행.
                while ((!file1.StreamReader.EndOfStream) || (!file2.StreamReader.EndOfStream))
                {
                    // Read 파일1 Line
                    if (false == file1.ReadLine()) { return false; }

                    // Read 파일2 Line
                    if (false == file2.ReadLine()) { return false; }

                    ////////////////////////////////////////////////////////////////////////////////
                    // 두 라인을 비교하여 다를경우, 콘솔 및 텍스트파일에 출력(공백라인 무시).
                    ////////////////////////////////////////////////////////////////////////////////
                    if ("" != file1.Line && "" != file2.Line && file1.Line != file2.Line)
                    {
                        // 텍스트파일을 초기화하지 않았을 경우(최초 1회 수행시) 텍스트파일 초기화, 체크, 객체 할당.
                        if (false == bInit)
                        {
                            // 텍스트파일 초기화.
                            if (false == resultFile.InitTextFile()) { return false; }

                            // 텍스트파일 체크.
                            if (false == resultFile.Check()) { return false; }

                            // StreamWriter 객체 할당
                            if (false == resultFile.SetStreamWriter()) { return false; }

                            // 초기화 후엔 true로 수정.
                            bInit = true;
                        }

                        // 비교결과를 포맷팅하여 StringBuilder에 저장.
                        StringBuilder data = ResultFormat(file1, file2);

                        // 비교결과를 콘솔에 출력.
                        Console.Write(data);

                        // 비교결과를 파일에 출력.
                        resultFile.WriteTextFile(data);
                    }
                    ////////////////////////////////////////////////////////////////////////////////
                }

                // StreamReader 객체 내부스트림 Close
                file1.CloseStreamReader();
                file2.CloseStreamReader();

                // 두 파일의 내용이 다른게 있을 경우(객체 할당을 했을 경우) 내부스트림 Close
                if (true == bInit)
                {
                    resultFile.CloseStreamWriter();
                }
                // 두 파일의 내용이 같을 경우
                else
                {
                    Console.WriteLine("Files are equals.");
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
        /// 출력할 내용을 포맷팅.
        /// </summary>
        /// <param name="file1">파일1의 PFile 객체</param>
        /// <param name="file2">파일2의 PFile 객체</param>
        /// <returns>true: 정상, false: 비정상.</returns>
        private static StringBuilder ResultFormat(PFile file1, PFile file2)
        {
            StringBuilder sbData = new StringBuilder();

            /*
                /// 2017.04.12.현재 양식 ///
                -----------------------------------------------------------------------------------
                [File1 (라인번호)]	파일1 내용
                [File2 (라인번호)]	파일2 내용
            */

            sbData.Append("-----------------------------------------------------------------------------------\r\n");
            sbData.Append("[File1 (");
            sbData.Append(file1.LineNum);
            sbData.Append(")]\t");
            sbData.Append(file1.Line);
            sbData.Append("\r\n");
            sbData.Append("[File2 (");
            sbData.Append(file2.LineNum);
            sbData.Append(")]\t");
            sbData.Append(file2.Line);
            sbData.Append("\r\n");

            return sbData;
        }

    }
}
