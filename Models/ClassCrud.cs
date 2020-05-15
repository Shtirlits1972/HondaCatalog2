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

        public static List<CarTypeInfo> GetListCarTypeInfo(string vin)
        {
            List<CarTypeInfo> list = null;

            if (!(String.IsNullOrEmpty(vin)) && (vin.Length == 17))
            {
                try
                {
                    string nfrmpf = vin.Substring(0, 11);
                    string nfrmseqepc = vin.Substring(8, 8);

                    int h = 0;

                    #region strCommand
                    string strCommand = "   SELECT DISTINCT " +
                                "   pmotyt.hmodtyp,  " +
                                " 	pmotyt.cmodnamepc, " +
                                " 	pmotyt.xcardrs, " +
                                " 	pmotyt.dmodyr, " +
                                " 	pmotyt.xgradefulnam, " +
                                " 	pmotyt.ctrsmtyp, " +
                                " 	pmotyt.cmftrepc, " +
                                "   pmotyt.carea " +
                                " FROM pmodlt " +
                                "     JOIN pmotyt " +
                                "         ON(pmodlt.cmodnamepc = pmotyt.cmodnamepc " +
                                "         AND pmodlt.dmodyr = pmotyt.dmodyr " +
                                "         AND pmodlt.xcardrs = pmotyt.xcardrs) " +
                                " 	,pmdldt " +
                                " WHERE(pmotyt.nfrmpf = 'JHMED73600S') " +
                                "     AND(pmotyt.nfrmseqepcstrt <= '0S205949') " +
                                "     AND(pmotyt.nfrmseqepcend >= '0S205949') " +
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
                        list = db.Query<CarTypeInfo>(strCommand, new { nfrmpf, nfrmseqepc }).ToList();
                    }
                }
                catch
                {

                }
            }

            return list;
        }
        public static List<ModelCar> GetModelCars()
        {
            List<ModelCar> list = null;
            string strCommand = "SELECT CONCAT(cmodnamepc,'_', dmodyr, '_',xcardrs,'_',cmftrepc) AS Id, " +
                " CONCAT(cmodnamepc,' ', dmodyr, ' ',xcardrs,' ',cmftrepc) as cmodnamepc, " + 
                " CONCAT(cmodnamepc,'-', dmodyr, '-',xcardrs,'-',cmftrepc) as seo_url FROM pmodlt;";
            try
            {
                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    list = db.Query<ModelCar>(strCommand).ToList();
                }
            }
            catch { }
            return list;
        }
        public static List<PartsGroup> GetPartsGroup(string hmodtyp)
        {
            List<PartsGroup> list = null;
            string strCommand = " SELECT CONCAT( pgrout.clangjap, '_', " +
                               "       pgrout.nplgrp) AS 'Id',    " + 
                               "       pgrout.xplgrp " + 
                               "   FROM pgrout " + 
                               "   WHERE pgrout.clangjap = '2' " + 
                               "   AND EXISTS(SELECT 1 " + 
                               "                  FROM pblokt, pblmtt " + 
                               "                 WHERE pblokt.nplgrp = pgrout.nplgrp " + 
                               "                   AND pblokt.npl = pblmtt.npl " + 
                               "                   AND pblokt.nplblk = pblmtt.nplblk " +
                               "                   AND pblmtt.hmodtyp = @hmodtyp ); ";

            try
            {
                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    list = db.Query<PartsGroup>(strCommand, new { hmodtyp }).ToList();
                }
            }
            catch { }
            return list;
        }
        public static List<header> GetHeaders()
        {
            List<header> list = new List<header>();

            header header1 = new header { fid = "hmodtyp", title = "Код типа автомобиля" };
            list.Add(header1);
            header header2 = new header { fid = "cmodnamepc", title = "Модель автомобиля" };
            list.Add(header2);
            header header3 = new header { fid = "xcardrs", title = "Кол-во дверей" };
            list.Add(header3);
            header header4 = new header { fid = "dmodyr", title = "Год выпуска" };
            list.Add(header4);
            header header5 = new header { fid = "xgradefulnam", title = "Класс" };
            list.Add(header5);
            header header6 = new header { fid = "ctrsmtyp", title = "Тип трансмиссии" };
            list.Add(header6);
            header header7 = new header { fid = "cmftrepc", title = "Страна производитель" };
            list.Add(header7);
            header header8 = new header { fid = "carea", title = "Страна рынок" };
            list.Add(header8);

            return list;
        }
        public static List<hotspots> GetHotspots(string illustrationnumber, string npl)
        {
            List<hotspots> list = null;
            try
            {
                #region strCommand
                string strCommand = " SELECT " +
                                "   h.recordid, " +
                                "   h.illustrationnumber, " +
                                "   h.partreferencenumber, " +
                                "   h.max_x, " +
                                "   h.max_y, " +
                                "   h.min_x, " +
                                "   h.min_y, " +
                                "   h.npl " +
                                "   FROM hotspots h " +
                                " WHERE h.illustrationnumber = @illustrationnumber " +
                                " AND h.npl = @npl;  ";
                #endregion

                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    list = db.Query<hotspots>(strCommand, new { illustrationnumber, npl }).ToList();
                }
            }
            catch {}

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
        public static List<Sgroups> GetSgroups(int hmodtyp, string nplgrp, string npl ="")
        {
            List<Sgroups> list = null;

            try
            {
                #region strCommand
                string strCommand = "  SELECT DISTINCT " +

                        "   CONCAT(pbldst.npl, '_', pblokt.nplblk) AS 'Id', " +
                        "           pbldst.xplblk AS  'name' " +
                        "      FROM pblokt " +
                        "           JOIN pbldst " +
                        "              ON pblokt.npl = pbldst.npl " +
                        "             AND pblokt.nplblk = pbldst.nplblk " +
                        "             and pbldst.clangjap = '2' ";

                        if(!String.IsNullOrEmpty(npl))
                        {
                            strCommand += $" and pbldst.npl = @npl ";   //  19SELKD1
                         }

                        strCommand += $"    WHERE pblokt.nplgrp = @nplgrp and " +
                        "                  (pblokt.nplblk in (SELECT DISTINCT pblmtt.nplblk " +
                        "                                FROM pblmtt " +
                        "                               WHERE ";

                if (!String.IsNullOrEmpty(npl))
                {
                    strCommand += $" (pblmtt.npl =  @npl ) AND ";
                }

                strCommand += $"   (pblmtt.hmodtyp = @hmodtyp   )) ) " +
                "  ORDER BY pbldst.xplblk ; ";
                #endregion

                using (IDbConnection db = new MySqlConnection(strConn))
                {
                    list = db.Query<Sgroups>(strCommand, new { hmodtyp, nplgrp, npl }).ToList();
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

    }
}
