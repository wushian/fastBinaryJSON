﻿using System;
using System.Collections.Generic;
using NUnit.Framework;
using System.Data;
using System.Collections;
using System.Threading;
using fastBinaryJSON;
using System.Collections.Specialized;
using System.Diagnostics;

namespace UnitTests
{
    public class Tests
    {
        #region [  helpers  ]
        static int count = 1000;
        static int tcount = 5;
        static DataSet ds = new DataSet();
        //static bool exotic = false;
        //static bool dsser = false;

        public enum Gender
        {
            Male,
            Female
        }

        public class colclass
        {
            public colclass()
            {
                items = new List<baseclass>();
                date = DateTime.Now;
                multilineString = @"
            AJKLjaskljLA
       ahjksjkAHJKS سلام فارسی
       AJKHSKJhaksjhAHSJKa
       AJKSHajkhsjkHKSJKash
       ASJKhasjkKASJKahsjk
            ";
                isNew = true;
                booleanValue = true;
                ordinaryDouble = 0.001;
                gender = Gender.Female;
                intarray = new int[5] { 1, 2, 3, 4, 5 };
            }
            public bool booleanValue { get; set; }
            public DateTime date { get; set; }
            public string multilineString { get; set; }
            public List<baseclass> items { get; set; }
            public decimal ordinaryDecimal { get; set; }
            public double ordinaryDouble { get; set; }
            public bool isNew { get; set; }
            public string laststring { get; set; }
            public Gender gender { get; set; }

            public DataSet dataset { get; set; }
            public Dictionary<string, baseclass> stringDictionary { get; set; }
            public Dictionary<baseclass, baseclass> objectDictionary { get; set; }
            public Dictionary<int, baseclass> intDictionary { get; set; }
            public Guid? nullableGuid { get; set; }
            public decimal? nullableDecimal { get; set; }
            public double? nullableDouble { get; set; }
            public Hashtable hash { get; set; }
            public baseclass[] arrayType { get; set; }
            public byte[] bytes { get; set; }
            public int[] intarray { get; set; }

        }

        public static colclass CreateObject(bool exotic, bool dataset)
        {
            var c = new colclass();

            c.booleanValue = true;
            c.ordinaryDecimal = 3;

            if (exotic)
            {
                c.nullableGuid = Guid.NewGuid();
                c.hash = new Hashtable();
                c.bytes = new byte[1024];
                c.stringDictionary = new Dictionary<string, baseclass>();
                c.objectDictionary = new Dictionary<baseclass, baseclass>();
                c.intDictionary = new Dictionary<int, baseclass>();
                c.nullableDouble = 100.003;

                if (dataset)
                    c.dataset = CreateDataset();
                c.nullableDecimal = 3.14M;

                c.hash.Add(new class1("0", "hello", Guid.NewGuid()), new class2("1", "code", "desc"));
                c.hash.Add(new class2("0", "hello", "pppp"), new class1("1", "code", Guid.NewGuid()));

                c.stringDictionary.Add("name1", new class2("1", "code", "desc"));
                c.stringDictionary.Add("name2", new class1("1", "code", Guid.NewGuid()));

                c.intDictionary.Add(1, new class2("1", "code", "desc"));
                c.intDictionary.Add(2, new class1("1", "code", Guid.NewGuid()));

                c.objectDictionary.Add(new class1("0", "hello", Guid.NewGuid()), new class2("1", "code", "desc"));
                c.objectDictionary.Add(new class2("0", "hello", "pppp"), new class1("1", "code", Guid.NewGuid()));

                c.arrayType = new baseclass[2];
                c.arrayType[0] = new class1();
                c.arrayType[1] = new class2();
            }


            c.items.Add(new class1("1", "1", Guid.NewGuid()));
            c.items.Add(new class2("2", "2", "desc1"));
            c.items.Add(new class1("3", "3", Guid.NewGuid()));
            c.items.Add(new class2("4", "4", "desc2"));

            c.laststring = "" + DateTime.Now;

            return c;
        }

        public class baseclass
        {
            public string Name { get; set; }
            public string Code { get; set; }
        }

        public class class1 : baseclass
        {
            public class1() { }
            public class1(string name, string code, Guid g)
            {
                Name = name;
                Code = code;
                guid = g;
            }
            public Guid guid { get; set; }
        }

        public class class2 : baseclass
        {
            public class2() { }
            public class2(string name, string code, string desc)
            {
                Name = name;
                Code = code;
                description = desc;
            }
            public string description { get; set; }
        }

        public class NoExt
        {
            [System.Xml.Serialization.XmlIgnore()]
            public string Name { get; set; }
            public string Address { get; set; }
            public int Age { get; set; }
            public baseclass[] objs { get; set; }
            public Dictionary<string, class1> dic { get; set; }
            public NoExt intern { get; set; }
        }

        public class Retclass
        {
            public object ReturnEntity { get; set; }
            public string Name { get; set; }
            public string Field1;
            public int Field2;
            public string ppp { get { return "sdfas df "; } }
            public DateTime date { get; set; }
            public DataTable ds { get; set; }
        }

        public struct Retstruct
        {
            public object ReturnEntity { get; set; }
            public string Name { get; set; }
            public string Field1;
            public int Field2;
            public string ppp { get { return "sdfas df "; } }
            public DateTime date { get; set; }
            public DataTable ds { get; set; }
        }

        private static long CreateLong(string s)
        {
            long num = 0;
            bool neg = false;
            foreach (char cc in s)
            {
                if (cc == '-')
                    neg = true;
                else if (cc == '+')
                    neg = false;
                else
                {
                    num *= 10;
                    num += (int)(cc - '0');
                }
            }

            return neg ? -num : num;
        }

        private static DataSet CreateDataset()
        {
            DataSet ds = new DataSet();
            for (int j = 1; j < 3; j++)
            {
                DataTable dt = new DataTable();
                dt.TableName = "Table" + j;
                dt.Columns.Add("col1", typeof(int));
                dt.Columns.Add("col2", typeof(string));
                dt.Columns.Add("col3", typeof(Guid));
                dt.Columns.Add("col4", typeof(string));
                dt.Columns.Add("col5", typeof(bool));
                dt.Columns.Add("col6", typeof(string));
                dt.Columns.Add("col7", typeof(string));
                ds.Tables.Add(dt);
                Random rrr = new Random();
                for (int i = 0; i < 100; i++)
                {
                    DataRow dr = dt.NewRow();
                    dr[0] = rrr.Next(int.MaxValue);
                    dr[1] = "" + rrr.Next(int.MaxValue);
                    dr[2] = Guid.NewGuid();
                    dr[3] = "" + rrr.Next(int.MaxValue);
                    dr[4] = true;
                    dr[5] = "" + rrr.Next(int.MaxValue);
                    dr[6] = "" + rrr.Next(int.MaxValue);

                    dt.Rows.Add(dr);
                }
            }
            return ds;
        }

        public class RetNestedclass
        {
            public Retclass Nested { get; set; }
        }
        #endregion

        [Test]
        public static void objectarray()
        {
            var o = new object[3] { 1, "sdfsdfs", DateTime.Now };
            var b = fastBinaryJSON.BJSON.ToBJSON(o);
            var s = fastBinaryJSON.BJSON.ToObject(b);
        }

        [Test]
        public static void ClassTest()
        {
            Retclass r = new Retclass();
            r.Name = "hello";
            r.Field1 = "dsasdF";
            r.Field2 = 2312;
            r.date = DateTime.Now;
            r.ds = CreateDataset().Tables[0];

            var b = fastBinaryJSON.BJSON.ToBJSON(r);

            var o = fastBinaryJSON.BJSON.ToObject(b);

            Assert.AreEqual(2312, (o as Retclass).Field2);
        }


        [Test]
        public static void StructTest()
        {
            Retstruct r = new Retstruct();
            r.Name = "hello";
            r.Field1 = "dsasdF";
            r.Field2 = 2312;
            r.date = DateTime.Now;
            r.ds = CreateDataset().Tables[0];

            var b = fastBinaryJSON.BJSON.ToBJSON(r);

            var o = fastBinaryJSON.BJSON.ToObject(b);

            Assert.AreEqual(2312, ((Retstruct)o).Field2);
        }

        [Test]
        public static void ParseTest()
        {
            Retclass r = new Retclass();
            r.Name = "hello";
            r.Field1 = "dsasdF";
            r.Field2 = 2312;
            r.date = DateTime.Now;
            r.ds = CreateDataset().Tables[0];

            var s = fastBinaryJSON.BJSON.ToBJSON(r);

            var o = fastBinaryJSON.BJSON.Parse(s);

            Assert.IsNotNull(o);
        }

        [Test]
        public static void StringListTest()
        {
            List<string> ls = new List<string>();
            ls.AddRange(new string[] { "a", "b", "c", "d" });

            var s = fastBinaryJSON.BJSON.ToBJSON(ls);

            var o = fastBinaryJSON.BJSON.ToObject(s);

            Assert.IsNotNull(o);
        }

        [Test]
        public static void IntListTest()
        {
            List<int> ls = new List<int>();
            ls.AddRange(new int[] { 1, 2, 3, 4, 5, 10 });

            var s = fastBinaryJSON.BJSON.ToBJSON(ls);

            var p = fastBinaryJSON.BJSON.Parse(s);
            var o = fastBinaryJSON.BJSON.ToObject(s); // long[] {1,2,3,4,5,10}

            Assert.IsNotNull(o);
        }

        [Test]
        public static void Variables()
        {
            var s = fastBinaryJSON.BJSON.ToBJSON(42);
            var o = fastBinaryJSON.BJSON.ToObject(s);
            Assert.AreEqual(o, 42);

            s = fastBinaryJSON.BJSON.ToBJSON("hello");
            o = fastBinaryJSON.BJSON.ToObject(s);
            Assert.AreEqual(o, "hello");
        }

        //[Test]
        //public static void SubClasses()
        //{

        //}

        //[Test]
        //public static void CasttoSomthing()
        //{

        //}

        //[Test]
        //public static void IgnoreCase()
        //{

        //}

        [Test]
        public static void Perftest()
        {
            string s = "123456";

            DateTime dt = DateTime.Now;

            for (int i = 0; i < 1000000; i++)
            {
                var o = CreateLong(s);
            }

            Console.WriteLine("convertlong (ms): " + DateTime.Now.Subtract(dt).TotalMilliseconds);

            dt = DateTime.Now;

            for (int i = 0; i < 1000000; i++)
            {
                var o = long.Parse(s);
            }

            Console.WriteLine("long.parse (ms): " + DateTime.Now.Subtract(dt).TotalMilliseconds);

            dt = DateTime.Now;

            for (int i = 0; i < 1000000; i++)
            {
                var o = Convert.ToInt64(s);
            }

            Console.WriteLine("convert.toint64 (ms): " + DateTime.Now.Subtract(dt).TotalMilliseconds);
        }

        [Test]
        public static void List_int()
        {
            List<int> ls = new List<int>();
            ls.AddRange(new int[] { 1, 2, 3, 4, 5, 10 });

            var s = fastBinaryJSON.BJSON.ToBJSON(ls);
            Console.WriteLine(s);
            var p = fastBinaryJSON.BJSON.Parse(s);
            var o = fastBinaryJSON.BJSON.ToObject<List<int>>(s);

            Assert.IsNotNull(o);
        }

        [Test]
        public static void Dictionary_String_RetClass()
        {
            Dictionary<string, Retclass> r = new Dictionary<string, Retclass>();
            r.Add("11", new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
            r.Add("12", new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
            var s = fastBinaryJSON.BJSON.ToBJSON(r);
            var o = fastBinaryJSON.BJSON.ToObject<Dictionary<string, Retclass>>(s);
            Assert.AreEqual(2, o.Count);
        }

        [Test]
        public static void Dictionary_String_RetClass_noextensions()
        {
            Dictionary<string, Retclass> r = new Dictionary<string, Retclass>();
            r.Add("11", new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
            r.Add("12", new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
            var s = fastBinaryJSON.BJSON.ToBJSON(r, new fastBinaryJSON.BJSONParameters { UseExtensions = false });
            var o = fastBinaryJSON.BJSON.ToObject<Dictionary<string, Retclass>>(s);
            Assert.AreEqual(2, o.Count);
        }

        [Test]
        public static void Dictionary_int_RetClass()
        {
            Dictionary<int, Retclass> r = new Dictionary<int, Retclass>();
            r.Add(11, new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
            r.Add(12, new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
            var s = fastBinaryJSON.BJSON.ToBJSON(r);
            var o = fastBinaryJSON.BJSON.ToObject<Dictionary<int, Retclass>>(s);
            Assert.AreEqual(2, o.Count);
        }

        [Test]
        public static void Dictionary_int_RetClass_noextensions()
        {
            Dictionary<int, Retclass> r = new Dictionary<int, Retclass>();
            r.Add(11, new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
            r.Add(12, new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
            var s = fastBinaryJSON.BJSON.ToBJSON(r, new fastBinaryJSON.BJSONParameters { UseExtensions = false });
            var o = fastBinaryJSON.BJSON.ToObject<Dictionary<int, Retclass>>(s);
            Assert.AreEqual(2, o.Count);
        }

        [Test]
        public static void Dictionary_Retstruct_RetClass()
        {
            Dictionary<Retstruct, Retclass> r = new Dictionary<Retstruct, Retclass>();
            r.Add(new Retstruct { Field1 = "111", Field2 = 1, date = DateTime.Now }, new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
            r.Add(new Retstruct { Field1 = "222", Field2 = 2, date = DateTime.Now }, new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
            var s = fastBinaryJSON.BJSON.ToBJSON(r);
            var o = fastBinaryJSON.BJSON.ToObject<Dictionary<Retstruct, Retclass>>(s);
            Assert.AreEqual(2, o.Count);
        }

        [Test]
        public static void Dictionary_Retstruct_RetClass_noextentions()
        {
            Dictionary<Retstruct, Retclass> r = new Dictionary<Retstruct, Retclass>();
            r.Add(new Retstruct { Field1 = "111", Field2 = 1, date = DateTime.Now }, new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
            r.Add(new Retstruct { Field1 = "222", Field2 = 2, date = DateTime.Now }, new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
            var s = fastBinaryJSON.BJSON.ToBJSON(r, new fastBinaryJSON.BJSONParameters { UseExtensions = false });
            var o = fastBinaryJSON.BJSON.ToObject<Dictionary<Retstruct, Retclass>>(s);
            Assert.AreEqual(2, o.Count);
        }

        [Test]
        public static void List_RetClass()
        {
            List<Retclass> r = new List<Retclass>();
            r.Add(new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
            r.Add(new Retclass { Field1 = "222", Field2 = 3, date = DateTime.Now });
            var s = fastBinaryJSON.BJSON.ToBJSON(r);
            var o = fastBinaryJSON.BJSON.ToObject<List<Retclass>>(s);
            Assert.AreEqual(2, o.Count);
        }

        [Test]
        public static void List_RetClass_noextensions()
        {
            List<Retclass> r = new List<Retclass>();
            r.Add(new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now });
            r.Add(new Retclass { Field1 = "222", Field2 = 3, date = DateTime.Now });
            var s = fastBinaryJSON.BJSON.ToBJSON(r, new fastBinaryJSON.BJSONParameters { UseExtensions = false });
            var o = fastBinaryJSON.BJSON.ToObject<List<Retclass>>(s);
            Assert.AreEqual(2, o.Count);
        }

        [Test]
        public static void FillObject()
        {
            NoExt ne = new NoExt();
            ne.Name = "hello";
            ne.Address = "here";
            ne.Age = 10;
            ne.dic = new Dictionary<string, class1>();
            ne.dic.Add("hello", new class1("asda", "asdas", Guid.NewGuid()));
            ne.objs = new baseclass[] { new class1("a", "1", Guid.NewGuid()), new class2("b", "2", "desc") };

            byte[] str = fastBinaryJSON.BJSON.ToBJSON(ne, new fastBinaryJSON.BJSONParameters { UseExtensions = false, UsingGlobalTypes = false });
            object dic = fastBinaryJSON.BJSON.Parse(str);
            object oo = fastBinaryJSON.BJSON.ToObject<NoExt>(str);

            NoExt nee = new NoExt();
            nee.intern = new NoExt { Name = "aaa" };
            fastBinaryJSON.BJSON.FillObject(nee, str);
        }

        [Test]
        public static void AnonymousTypes()
        {
            Console.WriteLine(".net version = " + Environment.Version);
            var q = new { Name = "asassa", Address = "asadasd", Age = 12 };
            byte[] sq = fastBinaryJSON.BJSON.ToBJSON(q, new fastBinaryJSON.BJSONParameters { EnableAnonymousTypes = true });
        }

        [Test]
        public static void Speed_Test_Deserialize()
        {

            Console.Write("fastbinaryjson deserialize");
            colclass c = CreateObject(false, false);
            double t = 0;
            for (int pp = 0; pp < tcount; pp++)
            {
                DateTime st = DateTime.Now;
                colclass deserializedStore;
                byte[] jsonText = fastBinaryJSON.BJSON.ToBJSON(c);
                for (int i = 0; i < count; i++)
                {
                    deserializedStore = fastBinaryJSON.BJSON.ToObject<colclass>(jsonText, new BJSONParameters { ParametricConstructorOverride = true });
                }
                t += DateTime.Now.Subtract(st).TotalMilliseconds;
                Console.Write("\t" + DateTime.Now.Subtract(st).TotalMilliseconds);
            }
            Console.WriteLine("\tAVG = " + t / tcount);
        }

        [Test]
        public static void Speed_Test_Serialize()
        {
            Console.Write("fastbinaryjson serialize");
            //fastBinaryJSON.BJSON.Parameters.UsingGlobalTypes = false;
            colclass c = CreateObject(false, false);
            double t = 0;
            for (int pp = 0; pp < tcount; pp++)
            {
                DateTime st = DateTime.Now;
                byte[] jsonText = null;
                for (int i = 0; i < count; i++)
                {
                    jsonText = fastBinaryJSON.BJSON.ToBJSON(c);
                }
                t += DateTime.Now.Subtract(st).TotalMilliseconds;
                Console.Write("\t" + DateTime.Now.Subtract(st).TotalMilliseconds);
            }
            Console.WriteLine("\tAVG = " + t / tcount);
        }

        [Test]
        public static void List_NestedRetClass()
        {
            List<RetNestedclass> r = new List<RetNestedclass>();
            r.Add(new RetNestedclass { Nested = new Retclass { Field1 = "111", Field2 = 2, date = DateTime.Now } });
            r.Add(new RetNestedclass { Nested = new Retclass { Field1 = "222", Field2 = 3, date = DateTime.Now } });
            var s = fastBinaryJSON.BJSON.ToBJSON(r);
            var o = fastBinaryJSON.BJSON.ToObject<List<RetNestedclass>>(s);
            Assert.AreEqual(2, o.Count);
        }

        [Test]
        public static void NullTest()
        {
            var s = fastBinaryJSON.BJSON.ToBJSON(null);
            Assert.AreEqual(s[0], fastBinaryJSON.TOKENS.NULL);
            var o = fastBinaryJSON.BJSON.ToObject(s);
            Assert.AreEqual(null, o);
        }

        [Test]
        public static void ZeroArray()
        {
            var s = fastBinaryJSON.BJSON.ToBJSON(new object[] { });
            var o = fastBinaryJSON.BJSON.ToObject(s);
            var a = o as object[];
            Assert.AreEqual(0, a.Length);
        }

        [Test]
        public static void GermanNumbers()
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("de");
            decimal d = 3.141592654M;
            var s = fastBinaryJSON.BJSON.ToBJSON(d);
            var o = fastBinaryJSON.BJSON.ToObject(s);
            Assert.AreEqual(d, (decimal)o);

            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en");
        }

        public class arrayclass
        {
            public int[] ints { get; set; }
            public string[] strs;
        }
        [Test]
        public static void ArrayTest()
        {
            arrayclass a = new arrayclass();
            a.ints = new int[] { 3, 1, 4 };
            a.strs = new string[] { "a", "b", "c" };
            var s = fastBinaryJSON.BJSON.ToBJSON(a);
            var o = fastBinaryJSON.BJSON.ToObject(s);
        }

        [Test]
        public static void Datasets()
        {
            var ds = CreateDataset();

            var s = fastBinaryJSON.BJSON.ToBJSON(ds);

            var o = fastBinaryJSON.BJSON.ToObject<DataSet>(s);

            Assert.AreEqual(typeof(DataSet), o.GetType());
            Assert.IsNotNull(o);
            Assert.AreEqual(2, o.Tables.Count);


            s = fastBinaryJSON.BJSON.ToBJSON(ds.Tables[0]);
            var oo = fastBinaryJSON.BJSON.ToObject<DataTable>(s);
            Assert.IsNotNull(oo);
            Assert.AreEqual(typeof(DataTable), oo.GetType());
            Assert.AreEqual(100, oo.Rows.Count);
        }

        [Test]
        public static void DynamicTest()
        {
            var obj = new { Name = "aaaaaa", Age = 10, dob = DateTime.Parse("2000-01-01 00:00:00"), inner = new { prop = 30 } };

            byte[] b = fastBinaryJSON.BJSON.ToBJSON(
                obj,
                new fastBinaryJSON.BJSONParameters { UseExtensions = false, EnableAnonymousTypes = true });
            dynamic d = fastBinaryJSON.BJSON.ToDynamic(b);
            var ss = d.Name;
            var oo = d.Age;
            var dob = d.dob;
            var inp = d.inner.prop;

            Assert.AreEqual("aaaaaa", ss);
            Assert.AreEqual(10, oo);
            Assert.AreEqual(30, inp);
            Assert.AreEqual(DateTime.Parse("2000-01-01 00:00:00"), dob);
        }

        public class diclist
        {
            public Dictionary<string, List<string>> d;
        }

        [Test]
        public static void DictionaryWithListValue()
        {
            diclist dd = new diclist();
            dd.d = new Dictionary<string, List<string>>();
            dd.d.Add("a", new List<string> { "1", "2", "3" });
            dd.d.Add("b", new List<string> { "4", "5", "7" });
            byte[] s = BJSON.ToBJSON(dd, new BJSONParameters { UseExtensions = false });
            var o = BJSON.ToObject<diclist>(s);
            Assert.AreEqual(3, o.d["a"].Count);

            s = BJSON.ToBJSON(dd.d, new BJSONParameters { UseExtensions = false });
            var oo = BJSON.ToObject<Dictionary<string, List<string>>>(s);
            Assert.AreEqual(3, oo["a"].Count);
            var ooo = BJSON.ToObject<Dictionary<string, string[]>>(s);
            Assert.AreEqual(3, ooo["b"].Length);
        }

        [Test]
        public static void HashtableTest()
        {
            Hashtable h = new Hashtable();
            h.Add(1, "dsjfhksa");
            h.Add("dsds", new class1());

            var s = BJSON.ToBJSON(h);

            var o = BJSON.ToObject<Hashtable>(s);
            Assert.AreEqual(typeof(Hashtable), o.GetType());
            Assert.AreEqual(typeof(class1), o["dsds"].GetType());
        }

        public class coltest
        {
            public string name;
            public NameValueCollection nv;
            public StringDictionary sd;
        }

        [Test]
        public static void SpecialCollections()
        {
            var nv = new NameValueCollection();
            nv.Add("1", "a");
            nv.Add("2", "b");
            var s = fastBinaryJSON.BJSON.ToBJSON(nv);
            var oo = fastBinaryJSON.BJSON.ToObject<NameValueCollection>(s);
            Assert.AreEqual("a", oo["1"]);
            var sd = new StringDictionary();
            sd.Add("1", "a");
            sd.Add("2", "b");
            s = fastBinaryJSON.BJSON.ToBJSON(sd);
            var o = fastBinaryJSON.BJSON.ToObject<StringDictionary>(s);
            Assert.AreEqual("b", o["2"]);

            coltest c = new coltest();
            c.name = "aaa";
            c.nv = nv;
            c.sd = sd;
            s = fastBinaryJSON.BJSON.ToBJSON(c);
            var ooo = fastBinaryJSON.BJSON.ToObject(s);
            Assert.AreEqual("a", (ooo as coltest).nv["1"]);
            Assert.AreEqual("b", (ooo as coltest).sd["2"]);
        }

        public enum enumt
        {
            A = 65,
            B = 90,
            C = 100
        }
        public class constch
        {
            public enumt e = enumt.B;
            public string Name = "aa";
            public const int age = 11;
        }

        [Test]
        public static void consttest()
        {
            var s = BJSON.ToBJSON(new constch());
            var o = BJSON.ToObject(s);
        }

        public class ignoreatt : Attribute
        {
        }

        public class ignore
        {
            public string Name { get; set; }
            [System.Xml.Serialization.XmlIgnore]
            public int Age1 { get; set; }
            [ignoreatt]
            public int Age2;
        }
        public class ignore1 : ignore
        {
        }

        [Test]
        public static void IgnoreAttributes()
        {
            var i = new ignore { Age1 = 10, Age2 = 20, Name = "aa" };
            var s = fastBinaryJSON.BJSON.ToBJSON(i);
            var o = fastBinaryJSON.BJSON.ToObject<ignore>(s);
            Assert.AreEqual(0, o.Age1);
            i = new ignore1 { Age1 = 10, Age2 = 20, Name = "bb" };
            var j = new BJSONParameters();
            j.IgnoreAttributes.Add(typeof(ignoreatt));
            s = fastBinaryJSON.BJSON.ToBJSON(i, j);
            var oo = fastBinaryJSON.BJSON.ToObject<ignore1>(s);
            Assert.AreEqual(0, oo.Age1);
            Assert.AreEqual(0, oo.Age2);
        }

        public class nondefaultctor
        {
            public nondefaultctor(int a)
            { age = a; }
            public int age;
        }

        [Test]
        public static void NonDefaultConstructor()
        {
            var o = new nondefaultctor(10);
            //fastBinaryJSON.BJSON.Parameters.ParametricConstructorOverride = true;
            var s = fastBinaryJSON.BJSON.ToBJSON(o);
            Console.WriteLine(s);
            var obj = fastBinaryJSON.BJSON.ToObject<nondefaultctor>(s, new BJSONParameters { ParametricConstructorOverride = true });
            Assert.AreEqual(10, obj.age);
            List<nondefaultctor> l = new List<nondefaultctor> { o, o, o };
            s = fastBinaryJSON.BJSON.ToBJSON(l);
            var obj2 = fastBinaryJSON.BJSON.ToObject<List<nondefaultctor>>(s, new BJSONParameters { ParametricConstructorOverride = true });
            Assert.AreEqual(3, obj2.Count);
            Assert.AreEqual(10, obj2[1].age);
            //fastBinaryJSON.BJSON.Parameters.ParametricConstructorOverride = false;
        }

        public class o1
        {
            public int o1int;
            public o2 o2obj;
            public o3 child;
        }
        public class o2
        {
            public int o2int;
            public o1 parent;
        }
        public class o3
        {
            public int o3int;
            public o2 child;
        }

        [Test]
        public static void CircularReferences()
        {
            var o = new o1 { o1int = 1, child = new o3 { o3int = 3 }, o2obj = new o2 { o2int = 2 } };
            o.o2obj.parent = o;
            o.child.child = o.o2obj;

            var s = BJSON.ToBJSON(o, new BJSONParameters());
            //Console.WriteLine(BJSON.Beautify(s));
            var p = BJSON.ToObject<o1>(s);
            Assert.AreEqual(p, p.o2obj.parent);
            Assert.AreEqual(p.o2obj, p.child.child);
        }

        public class lol
        {
            public List<List<object>> r;
        }
        public class lol2
        {
            public List<object[]> r;
        }
        [Test]
        public static void ListOfList()
        {
            var o = new List<List<object>> { new List<object> { 1, 2, 3 }, new List<object> { "aa", 3, "bb" } };
            var s = fastBinaryJSON.BJSON.ToBJSON(o);
            //Console.WriteLine(s);
            var i = fastBinaryJSON.BJSON.ToObject(s);
            var p = new lol { r = o };
            s = fastBinaryJSON.BJSON.ToBJSON(p);
            //Console.WriteLine(s);
            i = fastBinaryJSON.BJSON.ToObject(s);
            Assert.AreEqual(3, (i as lol).r[0].Count);

            var oo = new List<object[]> { new object[] { 1, 2, 3 }, new object[] { "a", 4, "b" } };
            s = fastBinaryJSON.BJSON.ToBJSON(oo);
            //Console.WriteLine(s);
            var ii = fastBinaryJSON.BJSON.ToObject(s);
            lol2 l = new lol2() { r = oo };

            s = fastBinaryJSON.BJSON.ToBJSON(l);
            //Console.WriteLine(s);
            var iii = fastBinaryJSON.BJSON.ToObject(s);
            Assert.AreEqual(3, (iii as lol2).r[0].Length);
        }

        public class Y
        {
            public byte[] BinaryData;
        }

        public class A
        {
            public int DataA;
            public A NextA;
        }

        public class B : A
        {
            public string DataB;
        }

        public class C : A
        {
            public DateTime DataC;
        }

        public class Root
        {
            public Y TheY;
            public List<A> ListOfAs = new List<A>();
            public string UnicodeText;
            public Root NextRoot;
            public int MagicInt { get; set; }
            public A TheReferenceA;

            public void SetMagicInt(int value)
            {
                MagicInt = value;
            }
        }

        [Test]
        public static void complexobject()
        {
            Root r = new Root();
            r.TheY = new Y { BinaryData = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF } };
            r.ListOfAs.Add(new A { DataA = 10 });
            r.ListOfAs.Add(new B { DataA = 20, DataB = "Hello" });
            r.ListOfAs.Add(new C { DataA = 30, DataC = DateTime.Today });
            r.UnicodeText = "Žlutý kůň ∊ WORLD";
            r.ListOfAs[2].NextA = r.ListOfAs[1];
            r.ListOfAs[1].NextA = r.ListOfAs[2];
            r.TheReferenceA = r.ListOfAs[2];
            r.NextRoot = r;


            Console.WriteLine("JSON:\n---\n{0}\n---", BJSON.ToBJSON(r));

            Console.WriteLine();

            Console.WriteLine("Nice JSON:\n---\n{0}\n---", BJSON.ToBJSON(BJSON.ToObject<Root>(BJSON.ToBJSON(r))));
        }


        public struct Foo
        {
            public string name;
        };

        public class Bar
        {
            public Foo foo;
        };

        [Test]
        public static void StructProperty()
        {
            Bar b = new Bar();
            b.foo = new Foo();
            b.foo.name = "Buzz";
            var json = fastBinaryJSON.BJSON.ToBJSON(b);
            Bar bar = fastBinaryJSON.BJSON.ToObject<Bar>(json);
        }

        public class readonlyclass
        {
            public readonlyclass()
            {
                ROName = "bb";
                Age = 10;
            }
            private string _ro = "aa";
            public string ROAddress { get { return _ro; } }
            public string ROName { get; private set; }
            public int Age { get; set; }
        }

        [Test]
        public static void ReadonlyTest()
        {
            var s = fastBinaryJSON.BJSON.ToBJSON(new readonlyclass(), new BJSONParameters { ShowReadOnlyProperties = true });
            var o = fastBinaryJSON.BJSON.ToObject(s);
        }

        public class InstrumentSettings
        {
            public string dataProtocol { get; set; }
            public static bool isBad { get; set; }
            public static bool isOk;

            public InstrumentSettings()
            {
                dataProtocol = "Wireless";
            }
        }

        [Test]
        public static void statictest()
        {
            var s = new InstrumentSettings();
            BJSONParameters pa = new BJSONParameters();
            pa.UseExtensions = false;
            InstrumentSettings.isOk = true;
            InstrumentSettings.isBad = true;

            var jsonStr = BJSON.ToBJSON(s, pa);

            var o = BJSON.ToObject<InstrumentSettings>(jsonStr);
        }

        public class arrayclass2
        {
            public int[] ints { get; set; }
            public string[] strs;
            public int[][] int2d { get; set; }
            public int[][][] int3d;
            public baseclass[][] class2d;
        }

        [Test]
        public static void ArrayTest2()
        {
            arrayclass2 a = new arrayclass2();
            a.ints = new int[] { 3, 1, 4 };
            a.strs = new string[] { "a", "b", "c" };
            a.int2d = new int[][] { new int[] { 1, 2, 3 }, new int[] { 2, 3, 4 } };
            a.int3d = new int[][][] {        new int[][] {
            new int[] { 0, 0, 1 },
            new int[] { 0, 1, 0 }
        },
        null,
        new int[][] {
            new int[] { 0, 0, 2 },
            new int[] { 0, 2, 0 },
            null
        }
    };
            a.class2d = new baseclass[][]{
        new baseclass[] {
            new baseclass () { Name = "a", Code = "A" },
            new baseclass () { Name = "b", Code = "B" }
        },
        new baseclass[] {
            new baseclass () { Name = "c" }
        },
        null
    };
            var s = BJSON.ToBJSON(a);
            var o = BJSON.ToObject<arrayclass2>(s);
            CollectionAssert.AreEqual(a.ints, o.ints);
            CollectionAssert.AreEqual(a.strs, o.strs);
            CollectionAssert.AreEqual(a.int2d[0], o.int2d[0]);
            CollectionAssert.AreEqual(a.int2d[1], o.int2d[1]);
            CollectionAssert.AreEqual(a.int3d[0][0], o.int3d[0][0]);
            CollectionAssert.AreEqual(a.int3d[0][1], o.int3d[0][1]);
            Assert.AreEqual(null, o.int3d[1]);
            CollectionAssert.AreEqual(a.int3d[2][0], o.int3d[2][0]);
            CollectionAssert.AreEqual(a.int3d[2][1], o.int3d[2][1]);
            CollectionAssert.AreEqual(a.int3d[2][2], o.int3d[2][2]);
            for (int i = 0; i < a.class2d.Length; i++)
            {
                var ai = a.class2d[i];
                var oi = o.class2d[i];
                if (ai == null && oi == null)
                {
                    continue;
                }
                for (int j = 0; j < ai.Length; j++)
                {
                    var aii = ai[j];
                    var oii = oi[j];
                    if (aii == null && oii == null)
                    {
                        continue;
                    }
                    Assert.AreEqual(aii.Name, oii.Name);
                    Assert.AreEqual(aii.Code, oii.Code);
                }
            }
        }

        [Test]
        public static void Dictionary_String_Object_WithList()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();

            dict.Add("C", new List<float>() { 1.1f, 2.2f, 3.3f });
            var json = BJSON.ToBJSON(dict);

            var des = BJSON.ToObject<Dictionary<string, List<float>>>(json);
            Assert.IsInstanceOf(typeof(List<float>), des["C"]);
        }

        [Test]
        public static void exotic_deserialize()
        {
            Console.WriteLine();
            Console.Write("fastjson deserialize");
            colclass c = CreateObject(true, true);
            var stopwatch = new Stopwatch();
            for (int pp = 0; pp < tcount; pp++)
            {
                colclass deserializedStore;
                byte[] jsonText = null;

                stopwatch.Restart();
                jsonText = BJSON.ToBJSON(c);
                //Console.WriteLine(" size = " + jsonText.Length);
                for (int i = 0; i < count; i++)
                {
                    deserializedStore = (colclass)BJSON.ToObject(jsonText);
                }
                stopwatch.Stop();
                Console.Write("\t" + stopwatch.ElapsedMilliseconds);
            }
        }

        [Test]
        public static void exotic_serialize()
        {
            Console.WriteLine();
            Console.Write("fastjson serialize");
            colclass c = CreateObject(true, true);
            var stopwatch = new Stopwatch();
            for (int pp = 0; pp < tcount; pp++)
            {
                byte[] jsonText = null;
                stopwatch.Restart();
                for (int i = 0; i < count; i++)
                {
                    jsonText = BJSON.ToBJSON(c);
                }
                stopwatch.Stop();
                Console.Write("\t" + stopwatch.ElapsedMilliseconds);
            }
        }

    }
}
