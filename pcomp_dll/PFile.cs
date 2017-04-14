using System;
using System.Text;
using System.IO;


namespace pfile
{
    /// <summary>
    /// 파일의 정보를 갖고 Init, Read, Write, Check 기능을 하는 클래스.
    /// </summary>
    /// <remarks>
    /// created : 2017.04.10.
    /// writer  : 장민수
    /// </remarks>
    public class PFile
    {
        public FileInfo FileInfo { get; private set; } // 파일의 정보
        public StreamReader StreamReader { get; private set; } // 문자를 읽는 TextReader
        public StreamWriter StreamWriter { get; private set; } // 문자를 쓰는 TextWriter
        public int TotalLine { get; private set; } // 파일 전체 라인수
        public int LineNum { get; set; } // 파일 현재 라인수
        public string Line { get; set; } // 파일 현재 라인의 내용


        /// <summary>
        /// 파일정보 객체를 생성하는 생성자.
        /// </summary>
        /// <remarks>
        /// created : 2017.04.10.
        /// writer  : 장민수
        /// </remarks>
        /// <param name="FullName">전체경로</param>
        public PFile(string FullName)
        {
            // 객체 할당.
            this.FileInfo = new FileInfo(FullName);
        }


        /// <summary>
        /// 파일의 내용을 담는 StreamReader Set.
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
                this.StreamReader = new StreamReader(this.FileInfo.FullName, Encoding.GetEncoding("euc-kr"));

                // 전체 라인 수
                TotalLine = File.ReadAllLines(this.FileInfo.FullName).Length;
            }
            catch (Exception e)
            {
                Console.WriteLine("예외 발생:{0}", e.Message);
                return false;
            }
            return true;
        }


        /// <summary>
        /// 파일에 내용을 쓰는 StreamWriter Set(기존 파일에 텍스트를 추가).
        /// </summary>
        /// <remarks>
        /// created : 2017.04.10.
        /// writer  : 장민수
        /// </remarks>
        /// <returns>true: 정상, false: 비정상.</returns>
        public bool SetStreamWriter()
        {
            try
            {
                // 기존 파일의 내용에 텍스트를 추가.
                this.StreamWriter = this.FileInfo.AppendText();
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
                if (!this.FileInfo.Directory.Exists)
                {
                    Console.WriteLine("해당 디렉토리가 존재하지 않습니다.({0})", this.FileInfo.FullName);
                    return false;
                }
                // 파일 존재여부 체크.
                if (!this.FileInfo.Exists)
                {
                    Console.WriteLine("해당 파일이 존재하지 않습니다.({0})", this.FileInfo.FullName);
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
                FileStream fsFile = this.FileInfo.OpenWrite();
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
        /// 파일의 한 줄을 읽어서 멤버변수 Line에 저장(공백라인 무시).
        /// </summary>
        /// <returns>true: 정상, false: 비정상.</returns>
        public bool ReadLine()
        {
            try
            {
                do
                {
                    // 다음 문자로 Peek
                    this.StreamReader.Peek();

                    // 파일의 끝이 아니면, 해당 라인을 읽어 string변수에 저장.
                    if (!this.StreamReader.EndOfStream)
                    {
                        // 라인수 카운트.
                        this.LineNum++;

                        // 한 라인을 읽어 string변수에 저장.
                        this.Line = this.StreamReader.ReadLine();

                        // 공백라인 무시.
                        if ("" != this.Line) { break; }

                        // 마지막라인이 공백라인이면
                        if (("" == this.Line) && this.StreamReader.EndOfStream)
                        {
                            // 라인 내용 초기화
                            EmptyLine();
                            break;
                        }
                    }
                    else
                    {
                        // 라인 내용 초기화
                        EmptyLine();
                    }
                } while (!this.StreamReader.EndOfStream); // 파일의 끝까지 반복하여 수행.
            }
            catch (Exception e)
            {
                Console.WriteLine("예외 발생:{0}", e.Message);
                return false;
            }
            return true;
        }


        /// <summary>
        /// 현재라인, 해당라인의 내용 초기화
        /// </summary>
        private void EmptyLine()
        {
            // 파일 현재라인 0으로 초기화.
            this.LineNum = 0;

            // '#####<EMPTY>#####' 문구로 셋팅
            this.Line = "#####<EMPTY>#####";
        }


        /// <summary>
        /// 파일에 text를 Write 함.
        /// </summary>
        /// <param name="data">파일에 기록할 내용</param>
        /// <returns>true: 정상, false: 비정상.</returns>
        public bool WriteLine(StringBuilder data)
        {
            try
            {
                StreamWriter.Write(data);

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
