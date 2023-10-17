using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ExcelDB
{
#if UNITY_EDITOR   
    /*
            구글 드라이브에 업로드 되어 있는 엑셀 파일의 공유 링크를
            http://www.innerweb.kr/page/g_download
            위의 사이트에서 변환해야 정삭적으로 다운로드 가능
     */

    public class ExcelDBSystem : MonoBehaviour
    {
        [SerializeField] public string URL;                                 // DB URL
        [SerializeField] public string ResourceFilePath;                    // 리소스 저장할 경로
        [SerializeField] public string DBFilePath = "DataTable.xlsx";       // 경로 + 저장할 이름
        [SerializeField] public ScriptableObject Type;
        ExcelFileInfo m_fileInfo;                                           // 엑셀 파일 정보
        ExcelFileInfo m_FileInfo
        {
            get
            {
                string path = "Assets/NewExcelInfo.asset";
                if (m_fileInfo == null)
                {
                    m_fileInfo = (ExcelFileInfo)GetAsset(typeof(ExcelFileInfo), path);
                }
                return m_fileInfo;
            }
            set
            {
                m_fileInfo = value;
            }
        }
        List<ExcelTable> m_excelData;
        private void Awake()
        {
            m_excelData = new List<ExcelTable>();
        }
        public void Solution()
        {
            if (!UpdateDB()) return;
            //if(m_fileInfo.LastUpdateTime == ) return; 엑셀 파일이 최신 파일인가?
            var tables = ReadDB();
            if (tables == null) return;
            for(int i =0; i< tables.Count; i++)
            {
                Generate(tables,i, Type.GetType());
            }
            
        }
        public bool UpdateDB()
        {
            if (DBFilePath == "")return false;
            {
                
            }
            m_excelData.Clear();
            // 파일 다운
            try
            {
                //if (!File.Exists(DBFilePath))
                //{
                    WebClient webClient = new WebClient();
                    webClient.DownloadFile(URL, DBFilePath);
                //}
            }
            // 예외 처리
            catch (WebException ex)
            {
                return false;
            }
            // 다운 성공
            return true;
        }
        public List<(string[][],string)> ReadDB()
        {
            // 초기화
            m_excelData.Clear();
            // 엑셀 파일 확인
            if (!File.Exists(DBFilePath)) 
            {
                Debug.LogError("엑셀 DB 파일이 존재하지 않습니다.");
                return null; 
            }
            // 엑셀 테이블 읽기
            var tables = ExcelReader.ReadDB(DBFilePath);
            return tables;
        }
        public bool Generate(List<(string[][],string)> tables,int index, Type type)
        {
            if (tables ==null || tables.Count ==0 || type == null) return false;
            if (ResourceFilePath == "" || ResourceFilePath == null) return false;

            // 데이터 화
            if (!(0 <= index && index < tables.Count)) return false;
            var table = tables[index];
            var num = m_excelData.FindIndex(x => x.TableName == table.Item2);
            if (num != -1) m_excelData.RemoveAt(num);
            m_excelData.Add(new ExcelTable(type, table.Item1, table.Item2));
                
            var data = m_excelData.Last();

            if (data.Field.All(x => x == null)) return false;
            // 저장 경로 생성
            string tablePath = Path.Combine(ResourceFilePath, data.TableName);
            // 디렉터리 확인
            if (!File.Exists(tablePath))
            {
                Directory.CreateDirectory(tablePath);
                AssetDatabase.Refresh();
                
            }
            // 에셋 파일 생성
            for (int row = 0; row < data.table.Length; row++)
            {
                var id = data.Field[0].Read(data.table[row][0]);
                var asset = GetAsset(type, Path.Combine(tablePath, id.ToString() + ".asset"));
                // 데이터 입력
                for (int column = 0; column < data.Field.Length; column++)
                {
                    var excelfield = data.Field[column];
                    if (excelfield == null) continue;
                    FieldInfo field = type.GetField(excelfield.FieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    field.SetValue(asset, excelfield.Read(data.table[row][column]));
                }
                UnityEditor.EditorUtility.SetDirty((UnityEngine.Object) asset);
            }
            AssetDatabase.SaveAssets();

            AssetDatabase.Refresh();
            return true;
        }
        object GetAsset(Type type, string path)
        {
            if (type == null) return null;
            // 에셋 파일 가져오기
            var asset = AssetDatabase.LoadAssetAtPath(path, type);
            // 없으면 생성
            if (asset == null)
            {
                var newData = ScriptableObject.CreateInstance(type);
                AssetDatabase.CreateAsset(newData, path);
                AssetDatabase.SaveAssets();
                asset = newData;
            }
            return asset;
        }
    }

    class ExcelTable
    {
        public ExcelTable(Type type,string[][] Table, string Name)
        {
            if (Table == null || type == null) return;

            // 테이블 이름 저장
            TableName = Name;
            Field = new IExcelField[Table[0].Length];
            // 필드 확인
            for (int column = 0; column < Table[0].Length; column++)
            {
                var fieldName = Table[0][column]?.ToString() ?? "";

                Field[column] = ExcelField.GetField(type,fieldName);
                if (Field[column] != null) Field[column].FieldName = fieldName;
            }

            // 테이블 복사
            table = new string[Table.Length-1][];
            for(int i=1; i<Table.Length; i++)
            {
                table[i - 1] = Table[i];
            }
            
        }
        
        public string TableName;            // 엑셀 테이블 이름
        public IExcelField[] Field;         // 테이블 필드
        public string[][] table;            // 테이블 데이터
    }
    static class ExcelReader
    {
        public static List<(string[][],string)> ReadDB(string DBFilePath)
        {
            // 엑셀 파일 확인
            if (!File.Exists(DBFilePath))
            {
                Debug.LogError("엑셀 DB 파일이 존재하지 않습니다.");
                return null;
            }
            var tables = new List<(string[][], string)>();
            // 파일 열기
            using (var stream = File.Open(DBFilePath, System.IO.FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var excel = reader.AsDataSet();
                    
                    // 테이블 단위로 읽어들이기
                    for (int tableNum = 0; tableNum < excel.Tables.Count; tableNum++)
                    {
                        var table = new string[excel.Tables[tableNum].Rows.Count][];
                        for (int row = 0; row < excel.Tables[tableNum].Rows.Count; row++)
                        {
                            // 데이터 테이블 작성
                            string[] newData = new string[excel.Tables[tableNum].Columns.Count];
                            for (int column = 0; column < excel.Tables[tableNum].Columns.Count; column++)
                                newData[column] = excel.Tables[tableNum].Rows[row][column]?.ToString() ?? "";
                            table[row] = newData;
                        }

                        tables.Add((table, excel.Tables[tableNum].TableName));
                    }
                }
            }
            return tables;
        }
    }
#endif
}