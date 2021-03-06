﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using HondaCatalog2.Models.Dto;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace HondaCatalog2.Models
{
    public class ClassCrud
    {
        private static string strConn = Ut.GetMySQLConnect();
        public static readonly string getLang = " SELECT DISTINCT lang.clangjap FROM lang  WHERE lang.code = @code ";

        public static List<VehiclePropArr> GetListCarTypeInfo(string vin)
        {
            List<VehiclePropArr> VehicleList = new List<VehiclePropArr>();

            if (!(String.IsNullOrEmpty(vin)) && (vin.Length == 17))
            {
                try
                {
                    string nfrmpf = vin.Substring(0, 11);
                    string nfrmseqepc = vin.Substring(9, 8);

                    int h = 0;

        //                    public int hmodtyp { get; set; }         //   hmodtyp 'Код типа автомобиля',
        //public string cmodnamepc { get; set; }   //  cmodnamepc  'Модель автомобиля',
        //public string xcardrs { get; set; }      //  xcardrs  'Кол-во дверей',
        //public string dmodyr { get; set; }       // dmodyr  'Год выпуска',
        //public string xgradefulnam { get; set; } //  xgradefulnam  Класс
        //public string ctrsmtyp { get; set; }     //   ctrsmtyp VARCHAR(3) NOT NULL COMMENT 'Тип трансмиссии',
        //public string cmftrepc { get; set; }     //  cmftrepc  'Страна производитель',
        //public string carea { get; set; }        //  carea  'Страна рынок',
        #region strCommand
        string strCommand = " SELECT DISTINCT " +
                                "  pmotyt.hmodtyp, " +
                                "  pmotyt.cmodnamepc, " +
                                "  pmotyt.xcardrs, " +
                                "  pmotyt.dmodyr, " +
                                "  pmotyt.xgradefulnam, " +
                                "  pmotyt.ctrsmtyp, " +
                                "  pmotyt.cmftrepc, " +
                                "  pmotyt.carea " +
                                " FROM pmodlt " +
                                "     JOIN pmotyt " +
                                "         ON(pmodlt.cmodnamepc = pmotyt.cmodnamepc " +
                                "         AND pmodlt.dmodyr = pmotyt.dmodyr " +
                                "         AND pmodlt.xcardrs = pmotyt.xcardrs), " +
                                " 	 pmdldt " +
                                " WHERE(pmotyt.nfrmpf = @nfrmpf ) " +
                                "     AND(pmotyt.nfrmseqepcstrt <= @nfrmseqepc ) " +
                                "     AND(pmotyt.nfrmseqepcend >= @nfrmseqepc ) " +
                                "         AND((NOT EXISTS(SELECT 1 " +
                                "                             FROM pmdldt " +
                                "                            WHERE pmdldt.cmodnamepc = pmodlt.cmodnamepc " +
                                "                              AND pmdldt.dmodyr = pmodlt.dmodyr " +
                                "                              AND pmdldt.xcardrs = pmodlt.xcardrs " +
                                "                              AND pmdldt.cmftrepc = pmodlt.cmftrepc) " +
                                "               AND pmodlt.dmodlnch < now()) " +
                                "          OR( " +
                                "                   EXISTS(SELECT 1 " +
                                "                         FROM pmdldt " +
                                "                        WHERE pmdldt.cmodnamepc = pmodlt.cmodnamepc " +
                                "                          AND pmdldt.dmodyr = pmodlt.dmodyr " +
                                "                          AND pmdldt.xcardrs = pmodlt.xcardrs " +
                                "                          AND pmdldt.cmftrepc = pmodlt.cmftrepc " +
                                "                          AND pmdldt.dmodlnch < now())))";
                    #endregion

                    using (IDbConnection db = new MySqlConnection(strConn))
                    {
                        List<CarTypeInfo> carTypeList = db.Query<CarTypeInfo>(strCommand, new { nfrmpf, nfrmseqepc }).ToList();


                        for(int i=0; i< carTypeList.Count; i++)
                        {
                            List<attributes> list = GetAttributes();

                            list[0].value = carTypeList[i].cmodnamepc;
                            list[1].value = carTypeList[i].xcardrs;
                            list[2].value = carTypeList[i].dmodyr;
                            list[3].value = carTypeList[i].xgradefulnam;
                            list[4].value = carTypeList[i].ctrsmtyp;
                            list[5].value = carTypeList[i].cmftrepc;
                            list[6].value = carTypeList[i].carea;

                            VehiclePropArr item = new VehiclePropArr { vehicle_id = carTypeList[i].hmodtyp.ToString(), model_name = carTypeList[i].cmodnamepc };
                            item.attributes = list;

                            VehicleList.Add(item);
                        }
                    }
                }
                catch(Exception ex)
                {
                    string Errror = ex.Message;
                    int o = 0;
                }
            }

            return VehicleList;
        }
        public static List<ModelCar> GetModelCars()
        {
            List<ModelCar> list = null;
            string strCommand = " SELECT DISTINCT cmodnamepc model_id, cmodnamepc model, " +
                                " REPLACE(cmodnamepc, ' ', '-')  seo_url FROM pmodlt; ";

            try
            {
                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    list = db.Query<ModelCar>(strCommand).ToList();
                }
            }
            catch(Exception ex)
            {
                string Error = ex.Message;
                int o = 0;
            }
            return list;
        }
        public static List<PartsGroup> GetPartsGroup(string vehicle_id, string code_lang = "EN")
        {
            List<PartsGroup> list = null;
            string strCommand = " SELECT DISTINCT " +
                                " CONCAT(pgrout.clangjap, '_', pgrout.nplgrp, '_', pblokt.npl) Id, " +
                                " pgrout.xplgrp name FROM pgrout " +
                                " LEFT JOIN pblokt ON pgrout.nplgrp = pblokt.nplgrp " +
                                " LEFT JOIN pblmtt ON pblokt.npl = pblmtt.npl " +
                                " AND pblokt.nplblk = pblmtt.nplblk " +
                                " WHERE pgrout.clangjap IN(SELECT DISTINCT " +
                                " lang.clangjap " +
                                " FROM lang " +
                                " WHERE lang.code = @code ) " +
                                " AND EXISTS(SELECT 1 " +
                                " FROM pblokt, pblmtt " +
                                " WHERE pblokt.nplgrp = pgrout.nplgrp " +
                                " AND pblokt.npl = pblmtt.npl " +
                                " AND pblokt.nplblk = pblmtt.nplblk " +
                                " AND pblmtt.hmodtyp = @hmodtyp) " +
                                " AND pblmtt.hmodtyp = @hmodtyp ";

            try
            {
                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    list = db.Query<PartsGroup>(strCommand, new { hmodtyp = vehicle_id, code = code_lang }).ToList();
                }

                for (int i = 0; i < list.Count; i++)
                {
                    List<Sgroups> listSgroups = GetSgroups(vehicle_id, list[i].Id, code_lang);
                    list[i].childs = listSgroups;
                }

            }
            catch (Exception ex)
            {
                string Error = ex.Message;
                int o = 0;
            }
            return list;
        }
        public static List<header> GetHeaders()
        {
            List<header> list = new List<header>();

            header header1 = new header { code = "hmodtyp", title = "Код типа автомобиля" };
            list.Add(header1);
            header header2 = new header { code = "cmodnamepc", title = "Модель автомобиля" };
            list.Add(header2);
            header header3 = new header { code = "xcardrs", title = "Кол-во дверей" };
            list.Add(header3);
            header header4 = new header { code = "dmodyr", title = "Год выпуска" };
            list.Add(header4);
            header header5 = new header { code = "xgradefulnam", title = "Класс" };
            list.Add(header5);
            header header6 = new header { code = "ctrsmtyp", title = "Тип трансмиссии" };
            list.Add(header6);
            header header7 = new header { code = "cmftrepc", title = "Страна производитель" };
            list.Add(header7);
            header header8 = new header { code = "carea", title = "Страна рынок" };
            list.Add(header8);

            return list;
        }
        public static List<SpareParts> GetSpareParts(string nplblk, int hmodtyp)
        {
            List<SpareParts> list = null;

            try
            {
                #region strCommand
                string strCommand = "  SELECT  " +
                                    "   p.hpartplblk, " +
                                    "   p2.xpartext, " +
                                    "   IFNULL(deskr.xpartrmrk, '') as xpartrmrk , " +
                                    "   p.nplblk, " +
                                    "   p.npl " +
                                    "   FROM " +
                                    "   pblpat p " +
                                    "   LEFT JOIN ppartt p2 " +
                                    "   ON p.npartgenu = p2.npartgenu " +
                                    "   LEFT JOIN pbprmt deskr ON " +
                                    "   p.hpartplblk = deskr.hpartplblk " +
                                    "   WHERE " +
                                    "   p2.clangjap = 2 AND " +
                                    "   p.nplblk = @nplblk AND " +
                                    "   p.hpartplblk IN " +
                                    " (SELECT pbpmtt.hpartplblk FROM pbpmtt " +
                                    "   WHERE pbpmtt.hmodtyp = @hmodtyp);  ";
                #endregion

                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    list = db.Query<SpareParts>(strCommand, new { nplblk, hmodtyp }).ToList();
                }
            }
            catch { }

            if (list != null)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].hotspots = GetHotspots(list[i].nplblk, list[i].npl);
                }
            }

            return list;
        }
        public static List<Filters> GetFilters(string modelId)
        {
            List<Filters> filters = new List<Filters>();

            try
            {
                string[] param = modelId.Split("_");

                if (param.Length == 4)
                {
                    string cmodnamepc = param[0];
                    string dmodyr = param[1];
                    string xcardrs = param[2];
                    string cmftrepc = param[3];

                    #region Тип трансмиссии
                    List<string> ctrsmtypList = new List<string>();
                    string ctrsmtypCom = "SELECT DISTINCT ctrsmtyp FROM " +
                                          " pmotyt p " +
                                          " WHERE " +
                                          $" p.cmodnamepc = '{cmodnamepc}' AND " +
                                          $" p.xcardrs = '{xcardrs}' AND " +
                                          $" p.dmodyr = '{dmodyr}' AND " +
                                          $" p.cmftrepc = '{cmftrepc}'; ";

                    using (IDbConnection db = new MySqlConnection(strConn))
                    {
                        ctrsmtypList = db.Query<string>(ctrsmtypCom, new { cmodnamepc, xcardrs, dmodyr, cmftrepc }).ToList();
                    }
                    Filters ctrsmtypF = new Filters { Id = "1", name = "Тип трансмиссии" };
                    List<values> ctrsmtypVal = new List<values>();

                    for (int i = 0; i < ctrsmtypList.Count; i++)
                    {
                        values v1 = new values { Id = ctrsmtypList[i], name = ctrsmtypList[i] };
                        ctrsmtypVal.Add(v1);
                    }

                    ctrsmtypF.values = ctrsmtypVal;
                    filters.Add(ctrsmtypF);
                    #endregion

                    #region Страна рынок
                    List<string> careaList = new List<string>();
                    string careaCom = "SELECT DISTINCT carea FROM " +
                                          " pmotyt p " +
                                          " WHERE " +
                                          " p.cmodnamepc = @cmodnamepc AND " +
                                          " p.xcardrs = @xcardrs AND " +
                                          " p.dmodyr = @dmodyr AND " +
                                          " p.cmftrepc = @cmftrepc; ";
                    using (IDbConnection db = new MySqlConnection(strConn))
                    {
                        careaList = db.Query<string>(careaCom, new { cmodnamepc, xcardrs, dmodyr, cmftrepc }).ToList();
                    }
                    Filters careaF = new Filters { Id = "2", name = "Страна рынок" };
                    List<values> careaVal = new List<values>();

                    for (int i = 0; i < careaList.Count; i++)
                    {
                        values v1 = new values { Id = careaList[i], name = careaList[i] };
                        careaVal.Add(v1);
                    }

                    careaF.values = careaVal;
                    filters.Add(careaF);
                    #endregion

                    #region  npl  'Каталог разборки',
                    List<string> nplList = new List<string>();
                    string nplCom = "SELECT DISTINCT npl FROM " +
                                          " pmotyt p " +
                                          " WHERE " +
                                          " p.cmodnamepc = @cmodnamepc AND " +
                                          " p.xcardrs = @xcardrs AND " +
                                          " p.dmodyr = @dmodyr AND " +
                                          " p.cmftrepc = @cmftrepc; ";
                    using (IDbConnection db = new MySqlConnection(strConn))
                    {
                        nplList = db.Query<string>(nplCom, new { cmodnamepc, xcardrs, dmodyr, cmftrepc }).ToList();
                    }
                    Filters nplF = new Filters { Id = "3", name = "Каталог разборки" };
                    List<values> nplVal = new List<values>();

                    for (int i = 0; i < nplList.Count; i++)
                    {
                        values v1 = new values { Id = nplList[i], name = nplList[i] };
                        nplVal.Add(v1);
                    }

                    nplF.values = nplVal;
                    filters.Add(nplF);
                    #endregion

                    #region  cmftrepc Страна производитель
                    //List<string> cmftrepcList = new List<string>();

                    //string cmftrepcCom = " SELECT DISTINCT cmftrepc FROM " +
                    //                      " pmotyt p " +
                    //                      " WHERE " +
                    //                      " p.cmodnamepc = @cmodnamepc AND " +
                    //                      " p.xcardrs = @xcardrs AND " +
                    //                      " p.dmodyr = @dmodyr AND " +
                    //                      " p.cmftrepc = @cmftrepc; ";

                    //using (IDbConnection db = new MySqlConnection(strConn))
                    //{
                    //    cmftrepcList = db.Query<string>(cmftrepcCom, new { cmodnamepc, xcardrs, dmodyr, cmftrepc }).ToList();
                    //}
                    //Filters cmftrepcF = new Filters { Id = "4", name = "Страна производитель" };
                    //List<values> cmftrepcVal = new List<values>();

                    //for (int i = 0; i < cmftrepcList.Count; i++)
                    //{
                    //    values v1 = new values { Id = cmftrepcList[i], name = cmftrepcList[i] };
                    //    cmftrepcVal.Add(v1);
                    //}

                    //cmftrepcF.values = cmftrepcVal;
                    //filters.Add(cmftrepcF);
                    #endregion

                    #region  nengnpf Код Двигателя
                    List<string> nengnpfList = new List<string>();
                    string nengnpfCom = "SELECT DISTINCT nengnpf FROM " +
                                          " pmotyt p " +
                                          " WHERE " +
                                          " p.cmodnamepc = @cmodnamepc AND " +
                                          " p.xcardrs = @xcardrs AND " +
                                          " p.dmodyr = @dmodyr AND " +
                                          " p.cmftrepc = @cmftrepc; ";
                    using (IDbConnection db = new MySqlConnection(strConn))
                    {
                        nengnpfList = db.Query<string>(nengnpfCom, new { cmodnamepc, xcardrs, dmodyr, cmftrepc }).ToList();
                    }
                    Filters nengnpfF = new Filters { Id = "5", name = "Код Двигателя" };
                    List<values> nengnpfVal = new List<values>();

                    for (int i = 0; i < nengnpfList.Count; i++)
                    {
                        values v1 = new values { Id = nengnpfList[i], name = nengnpfList[i] };
                        nengnpfVal.Add(v1);
                    }

                    nengnpfF.values = nengnpfVal;
                    filters.Add(nengnpfF);
                    #endregion
                }
            }
            catch { }

            return filters;
        }
        public static List<Sgroups> GetSgroups(string vehicle_id, string group_id, string code_lang = "EN")
        {
            List<Sgroups> list = null;

            string[] strArr = group_id.Split("_");

            string nplgrp = strArr[1];
            string npl = strArr[2];

            try
            {
                #region strCommand
                string strCommand = "  SELECT DISTINCT " +
                                    "   CONCAT(pbldst.npl, '_', pblokt.nplblk) node_id , " +
                                    "   pbldst.xplblk  name, " +
                                    "   pblokt.nplblk image_id, " +
                                    " '.png' image_ext " +
                                    "      FROM pblokt " +
                                    "           JOIN pbldst " +
                                    "              ON pblokt.npl = pbldst.npl " +
                                    "             AND pblokt.nplblk = pbldst.nplblk " +
                                    $"             and pbldst.clangjap IN ({getLang}) " +
                                    $"  and pbldst.npl = @npl " +
                                    $"  WHERE pblokt.nplgrp = @nplgrp and " +
                                    "   (pblokt.nplblk in (SELECT DISTINCT pblmtt.nplblk " +
                                    "   FROM pblmtt   WHERE " + 
                                    $" (pblmtt.npl =  @npl ) AND " +
                                    $"   (pblmtt.hmodtyp = @hmodtyp   )) ) " +
                                    "  ORDER BY pbldst.xplblk ; ";
                #endregion

                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    list = db.Query<Sgroups>(strCommand, new { hmodtyp = vehicle_id, nplgrp, npl, code = code_lang }).ToList();
                }
            }
            catch(Exception ex)
            {
                string Errror = ex.Message;
                int y = 0;
            }


            return list;
        }
        public static List<CarTypeInfo> GetListCarTypeInfoFilterCars(string modelId, string ctrsmtyp, string carea, string nengnpf)
        {
            List<CarTypeInfo> list = null;

            string [] param = modelId.Split("_", StringSplitOptions.None);

            if(param.Length == 4)
            {
                string cmodnamepc = param[0];
                string dmodyr = param[1];
                string xcardrs = param[2];
                string cmftrepc = param[3];

                try
                {

                    #region strCommand
                    string strCommand = "   SELECT DISTINCT " +
                                        "   p.hmodtyp,  " +
                                        " 	p.cmodnamepc, " +
                                        " 	p.xcardrs, " +
                                        " 	p.dmodyr, " +
                                        " 	p.xgradefulnam, " +
                                        " 	p.ctrsmtyp, " +
                                        " 	p.cmftrepc, " +
                                        "   p.carea " +
                                        "   FROM pmotyt p " +
                                        "     WHERE " +
                                        "    p.cmodnamepc = @cmodnamepc " +
                                        "    AND p.dmodyr = @dmodyr " +
                                        "    AND p.xcardrs = @xcardrs " +
                                        "    AND p.cmftrepc = @cmftrepc " +
                                        "    AND p.ctrsmtyp = @ctrsmtyp " +
                                        "    AND p.carea = @carea " +
                                        "    AND p.nengnpf = @nengnpf; ";
                    #endregion

                    using (IDbConnection db = new MySqlConnection(strConn))
                    {
                        list = db.Query<CarTypeInfo>(strCommand, new { cmodnamepc, dmodyr, xcardrs, cmftrepc, ctrsmtyp, carea,  nengnpf }).ToList();
                    }
                }
                catch(Exception ex)
                {
                    string Errror = ex.Message;
                    int y = 0;
                }
            }
            return list;
        }
        public static List<attributes> GetAttributes()
        {
            List<attributes> list = new List<attributes>();

            attributes cmodnamepc = new attributes { code = "cmodnamepc", name = "Модель автомобиля", value = "" };
            list.Add(cmodnamepc);

            attributes xcardrs = new attributes { code = "xcardrs", name = "Кол-во дверей", value = "" };
            list.Add(xcardrs);

            attributes dmodyr = new attributes { code = "dmodyr", name = "Год выпуска", value = "" };
            list.Add(dmodyr);

            attributes xgradefulnam = new attributes { code = "xgradefulnam", name = "Класс", value = "" };
            list.Add(xgradefulnam);

            attributes ctrsmtyp = new attributes { code = "ctrsmtyp", name = "Тип трансмиссии", value = "" };
            list.Add(ctrsmtyp);

            attributes cmftrepc = new attributes { code = "cmftrepc", name = "Страна производитель", value = "" };
            list.Add(cmftrepc);

            attributes carea = new attributes { code = "carea", name = "Страна, рынок", value = "" };
            list.Add(carea);

            return list;
        }
        public static VehiclePropArr GetVehiclePropArr(string vehicle_id)
        {
            VehiclePropArr model = null;

            try
            {
                #region strCommand
                string strCommand = "  SELECT DISTINCT " +
                                    "  p.hmodtyp, " +
                                    "  p.cmodnamepc, " +
                                    "  p.xcardrs, " +
                                    "  p.dmodyr, " +
                                    "  p.xgradefulnam, " +
                                    "  p.ctrsmtyp, " +
                                    "  p.cmftrepc, " +
                                    "  p.carea " +
                                    "  FROM pmotyt p " +
                                    "  WHERE p.hmodtyp = @vehicle_id LIMIT 1; ";
                #endregion

                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    CarTypeInfo carType = db.Query<CarTypeInfo>(strCommand, new { vehicle_id }).FirstOrDefault();

                    List<attributes> list = GetAttributes();

                    list[0].value = carType.cmodnamepc;
                    list[1].value = carType.xcardrs;
                    list[2].value = carType.dmodyr;
                    list[3].value = carType.xgradefulnam;
                    list[4].value = carType.ctrsmtyp;
                    list[5].value = carType.cmftrepc;
                    list[6].value = carType.carea;

                    model = new VehiclePropArr { vehicle_id = vehicle_id, model_name = carType.cmodnamepc };
                    model.attributes = list;
                }
            }
            catch (Exception ex)
            {
                string Errror = ex.Message;
                int y = 0;
            }

            return model;
        }
//==========================================================================================================================================
        public static List<lang> GetLang()
        {
            List<lang> list = new List<lang>();
            string strCommand = "   SELECT code, name, is_default FROM lang; ";

            try
            {
                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    list = db.Query<lang>(strCommand).ToList();
                }
            }
            catch (Exception ex)
            {
                string Error = ex.Message;
                int o = 0;
            }
            return list;
        }
        public static List<string> GetWmi()
        {
            List<string> list = new List<string>();
            string strCommand = " SELECT value FROM wmi; ";

            try
            {
                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    list = db.Query<string>(strCommand).ToList();
                }
            }
            catch (Exception ex)
            {
                string Error = ex.Message;
                int o = 0;
            }
            return list;
        }
        public static List<node> GetNodes(string[] codesArr = null, string[] node_idsArr = null)
        {
            List<node> list = new List<node>();
            string codes = null;
            string node_ids = null;

            if (codesArr != null && codesArr.Length > 0)
            {
                codes = string.Empty;

                for (int i = 0; i < codesArr.Length; i++)
                {
                    if (i == 0)
                    {
                        codes += codesArr[i];
                    }
                    else
                    {
                        codes += "," + codesArr[i];
                    }

                }
            }


            if (node_idsArr != null && node_idsArr.Length > 0)
            {
                node_ids = string.Empty;

                for (int i = 0; i < node_idsArr.Length; i++)
                {
                    if (i == 0)
                    {
                        node_ids += node_idsArr[i];
                    }
                    else
                    {
                        node_ids += "," + node_idsArr[i];
                    }
                }
            }

            string strCommand = " SELECT nt.code, nt.group name, nt.node_ids FROM " +
                                " nodes_tb nt ";

            if (!String.IsNullOrEmpty(codes) || !String.IsNullOrEmpty(node_ids))
            {
                strCommand += " WHERE";
            }

            if (!String.IsNullOrEmpty(codes))
            {
                strCommand += $"  nt.code IN  ({codes}) ";
            }

            if (!String.IsNullOrEmpty(codes) && !String.IsNullOrEmpty(node_ids))
            {
                strCommand += " OR ";
            }

            if (!String.IsNullOrEmpty(node_ids))
            {
                strCommand += $"  nt.node_ids IN  ({node_ids}) ";
            }

            try
            {
                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    list = db.Query<node>(strCommand).ToList();
                }
            }
            catch (Exception ex)
            {
                string Error = ex.Message;
                int o = 0;
            }
            return list;
        }
        public static DetailsInNode GetDetailsInNode(string vehicle_id, string node_id, string lang = "EN")
        {
            string npl = node_id.Substring(0, node_id.IndexOf("_"));
            string nplblk = node_id.Substring(node_id.IndexOf("_")+1, node_id.Length - (node_id.IndexOf("_")+1));

            DetailsInNode detailsInNode = new DetailsInNode { node_id = node_id };

            string strCommand = " SELECT DISTINCT pbldst.xplblk " +
                                " FROM " +
                                " pbldst " +
                                " WHERE " +
                                " pbldst.npl = @npl " +
                                " AND pbldst.nplblk = @nplblk " +
                                $" AND pbldst.clangjap IN ({getLang}) " +
                                " LIMIT 1; ";

            string strCommDeatil = " SELECT p.hpartplblk number, " +
            " p2.xpartext name " +
            " FROM pblpat p " +
            " LEFT JOIN ppartt p2 " +
            " ON p.npartgenu = p2.npartgenu " +
            " LEFT JOIN pbprmt deskr ON " +
            " p.hpartplblk = deskr.hpartplblk " +
            $" WHERE p2.clangjap IN ({getLang}) AND " +
            " p.nplblk = @nplblk AND " +
            " p.hpartplblk IN " +
            " (SELECT pbpmtt.hpartplblk FROM pbpmtt " +
            " WHERE pbpmtt.hmodtyp = @vehicle_id ); ";


            string strCommImages = " SELECT DISTINCT pbldst.nplblk id, '.png' ext " +
                                    " FROM pbldst WHERE " +
                                    " pbldst.npl = @npl " +
                                    " AND pbldst.nplblk = @nplblk " +
                                   $" AND pbldst.clangjap IN ({getLang}) ; ";

            try
            {
                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    detailsInNode.name = db.Query<string>(strCommand, new { npl, nplblk, code = lang }).FirstOrDefault();
                    detailsInNode.parts = db.Query<Detail>(strCommDeatil, new { nplblk, vehicle_id, code = lang }).ToList();
                    detailsInNode.images = db.Query<images>(strCommImages, new { npl, nplblk, code = lang }).ToList();
                }

                List<hotspots> hotspots = GetHotspots(nplblk, npl );

                for (int i = 0; i < detailsInNode.parts.Count; i++)
                {
                    detailsInNode.parts[i].hotspots = hotspots;
                }
            }
            catch (Exception ex)
            {
                string Error = ex.Message;
                int y = 0;
            }

            return detailsInNode;
        }
        public static List<hotspots> GetHotspots(string illustrationnumber, string npl)
        {
            List<hotspots> list = null;
            try
            {
                #region strCommand
                string strCommand = " SELECT " +
                                "   h.recordid hotspot_id, " +
                                "   h.illustrationnumber image_id, " +
                                "   h.max_x x2, " +
                                "   h.max_y y2, " +
                                "   h.min_x x1, " +
                                "   h.min_y y1  " +
                                "   FROM hotspots h " +
                                " WHERE h.illustrationnumber = @illustrationnumber " +
                                " AND h.npl = @npl;  ";
                #endregion
                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    list = db.Query<hotspots>(strCommand, new { illustrationnumber, npl }).ToList();
                }
            }
            catch (Exception ex)
            {
                string Errror = ex.Message;
                int o = 0;
            }

            return list;
        }
    }
}
