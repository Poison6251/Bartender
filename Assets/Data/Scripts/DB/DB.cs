using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace M_DB 
{
    public interface IQueryableToDB
    {
        public uint ID { get; }
    }
    public abstract class DB<T> : ScriptableObject where T : IQueryableToDB
    {
        [SerializeField]
        protected List<T> list;                 // 중복 데이터 입력 가능
        protected Dictionary<uint, T> db;       // DB
        public List<T> GetList
        {
            get
            {
                if(list == null) list = new List<T>();
                return list.ToList();
            }
        }
        public abstract void AddItem(T item);
        public abstract void RemoveItem(T item);
        public bool TryGetItemData(uint ID, out T itemData)
        {
            if (db == null)
            {
                db = new Dictionary<uint, T>();
                list.ForEach(x => db.Add(x.ID, x));
            }

            return db.TryGetValue(ID, out itemData);
        }

    }
    public class EditableDB<T> : DB<T> where T : IQueryableToDB
    {
        // DB 요소 수정 기능
        public override void AddItem(T item)
        {
            if (item == null) return;
            NotNullList();
            list.Add(item);
        }
        public override void RemoveItem(T item)
        {
            NotNullList();
            list.Remove(item);
        }
        private void NotNullList()
        {
            if (list == null)
            {
                list = new List<T>();
            }
        }
        public void Clear()
        {
            NotNullList();
            list.Clear();
        }
    }
}

