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
            ���� ����̺꿡 ���ε� �Ǿ� �ִ� ���� ������ ���� ��ũ��
            http://www.innerweb.kr/page/g_download
            ���� ����Ʈ���� ��ȯ�ؾ� ���������� �ٿ�ε� ����
     */

    public class ExcelDBSystem : MonoBehaviour
    {
        [SerializeField] public string URL;                                 // DB URL
        [SerializeField] public string ResourceFilePath;                    // ���ҽ� ������ ���
        [SerializeField] public string DBFilePath = "DataTable.xlsx";       // ��� + ������ �̸�
        [SerializeField] public ScriptableObject Type;
        ExcelFileInfo m_fileInfo;                                           // ���� ���� ����
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
            //if(m_fileInfo.LastUpdateTime == ) return; ���� ������ �ֽ� �����ΰ�?
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
            // ���� �ٿ�
            try
            {
                //if (!File.Exists(DBFilePath))
                //{
                    WebClient webClient = new WebClient();
                    webClient.DownloadFile(URL, DBFilePath);
                //}
            }
            // ���� ó��
            catch (WebException ex)
            {
                return false;
            }
            // �ٿ� ����
            return true;
        }
        public List<(string[][],string)> ReadDB()
        {
            // �ʱ�ȭ
            m_excelData.Clear();
            // ���� ���� Ȯ��
            if (!File.Exists(DBFilePath)) 
            {
                Debug.LogError("���� DB ������ �������� �ʽ��ϴ�.");
                return null; 
            }
            // ���� ���̺� �б�
            var tables = ExcelReader.ReadDB(DBFilePath);
            return tables;
        }
        public bool Generate(List<(string[][],string)> tables,int index, Type type)
        {
            if (tables ==null || tables.Count ==0 || type == null) return false;
            if (ResourceFilePath == "" || ResourceFilePath == null) return false;

            // ������ ȭ
            if (!(0 <= index && index < tables.Count)) return false;
            var table = tables[index];
            var num = m_excelData.FindIndex(x => x.TableName == table.Item2);
            if (num != -1) m_excelData.RemoveAt(num);
            m_excelData.Add(new ExcelTable(type, table.Item1, table.Item2));
                
            var data = m_excelData.Last();

            if (data.Field.All(x => x == null)) return false;
            // ���� ��� ����
            string tablePath = Path.Combine(ResourceFilePath, data.TableName);
            // ���͸� Ȯ��
            if (!File.Exists(tablePath))
            {
                Directory.CreateDirectory(tablePath);
                AssetDatabase.Refresh();
                
            }
            // ���� ���� ����
            for (int row = 0; row < data.table.Length; row++)
            {
                var id = data.Field[0].Read(data.table[row][0]);
                var asset = GetAsset(type, Path.Combine(tablePath, id.ToString() + ".asset"));
                // ������ �Է�
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
            // ���� ���� ��������
            var asset = AssetDatabase.LoadAssetAtPath(path, type);
            // ������ ����
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

            // ���̺� �̸� ����
            TableName = Name;
            Field = new IExcelField[Table[0].Length];
            // �ʵ� Ȯ��
            for (int column = 0; column < Table[0].Length; column++)
            {
                var fieldName = Table[0][column]?.ToString() ?? "";

                Field[column] = ExcelField.GetField(type,fieldName);
                if (Field[column] != null) Field[column].FieldName = fieldName;
            }

            // ���̺� ����
            table = new string[Table.Length-1][];
            for(int i=1; i<Table.Length; i++)
            {
                table[i - 1] = Table[i];
            }
            
        }
        
        public string TableName;            // ���� ���̺� �̸�
        public IExcelField[] Field;         // ���̺� �ʵ�
        public string[][] table;            // ���̺� ������
    }
    static class ExcelReader
    {
        public static List<(string[][],string)> ReadDB(string DBFilePath)
        {
            // ���� ���� Ȯ��
            if (!File.Exists(DBFilePath))
            {
                Debug.LogError("���� DB ������ �������� �ʽ��ϴ�.");
                return null;
            }
            var tables = new List<(string[][], string)>();
            // ���� ����
            using (var stream = File.Open(DBFilePath, System.IO.FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var excel = reader.AsDataSet();
                    
                    // ���̺� ������ �о���̱�
                    for (int tableNum = 0; tableNum < excel.Tables.Count; tableNum++)
                    {
                        var table = new string[excel.Tables[tableNum].Rows.Count][];
                        for (int row = 0; row < excel.Tables[tableNum].Rows.Count; row++)
                        {
                            // ������ ���̺� �ۼ�
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