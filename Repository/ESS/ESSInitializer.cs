using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using Repository.ESS.Domain;

namespace Repository.ESS
{
    public class ESSInitializer : CreateDatabaseIfNotExists<ESSContext>
    {
        private readonly ESSContext db = new ESSContext();

        public ESSInitializer()
        {
            this.Seed(db);
        }

        protected override void Seed(ESSContext db)
        {
            try
            {
                if (true)
                {
                    List<Role> roles = InitializeRoles();
                    roles.ForEach(x => db.Roles.Add(x));

                    List<Orgin> orgins = InitializeOrgin();
                    orgins.ForEach(x => db.Orgins.Add(x));

                    List<Account> accounts = InitializeAccounts(roles, orgins);
                    accounts.ForEach(x => db.Accounts.Add(x));

                    List<Bulletin> bulletins = InitializeBulletin(orgins);
                    bulletins.ForEach(x => db.Bulletins.Add(x));

                    List<AlartType> alartTypes = InitializeAlartType();
                    alartTypes.ForEach(x => db.AlartTypes.Add(x));

                    List<Station> stations = InitializeStation();
                    stations.ForEach(x => db.Stations.Add(x));

                    List<Alart> alarts = InitializeAlart(alartTypes, stations);
                    alarts.ForEach(x => db.Alarts.Add(x));

                    db.SaveChanges();
                }
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            catch (DbEntityValidationException ex)
            {
                var entityError = ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage);
                var getFullMessage = string.Join(";", entityError);
                var exceptionMessage = string.Concat(ex.Message, "errors are:", getFullMessage);
            }
        }

        #region  Initialize

        private List<Role> InitializeRoles()
        {
            List<Role> roles = new List<Role>()
            {
                new Role() { Type = 0 },
                new Role() { Type = 1 },
                new Role() { Type = 2 }
            };
            return roles;
        }
        private List<Orgin> InitializeOrgin()
        {
            List<Orgin> orgins = new List<Orgin>
            {
                new Orgin()
                {
                    OrginName = "總管理中心"
                },
                new Orgin()
                {
                    OrginName = "訪客"
                }
            };
            return orgins;
        }

        private List<Account> InitializeAccounts(List<Role> roles, List<Orgin> orgins)
        {
            List<Account> accounts = new List<Account>
            {
                new Account()
                {
                    UserName = "Admin",
                    Password = "1234",
                    Tel = "0900000000",
                    Email = "2100303134@gm.kuas.edu.tw",
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Disabled = false,
                    IsApproved = true,
                    IsLocked = false,
                    PasswordFailureCount = 0,
                    Role = roles.Where(x => x.Type == 0).FirstOrDefault(),
                   Orgin=orgins.Where(x => x.OrginName=="總管理中心").FirstOrDefault()
                },
                  new Account()
                {
                    UserName = "Guest",
                    Password = "Guest",
                    Tel = "0900000000",
                    Email = "2100303134@gm.kuas.edu.tw",
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Disabled = false,
                    IsApproved = true,
                    IsLocked = false,
                    PasswordFailureCount = 0,
                    Role = roles.Where(x => x.Type == 2).FirstOrDefault(),
                   Orgin=orgins.Where(x => x.OrginName=="訪客").FirstOrDefault()
                },
                new Account()
                {
                    UserName = "User1",
                    Password = "1234",
                    Tel = "0900000000",
                    Email = "2100303134@gm.kuas.edu.tw",
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Disabled = true,
                    IsApproved = true,
                    IsLocked = false,
                    PasswordFailureCount = 0,
                    Role = roles.Where(x => x.Type == 1).FirstOrDefault(),
                   Orgin=orgins.Where(x => x.OrginName=="總管理中心").FirstOrDefault()
                },
                new Account()
                {
                    UserName = "User2",
                    Password = "1234",
                    Tel = "0900000000",
                    Email = "2100303134@gm.kuas.edu.tw",
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Disabled = true,
                    IsApproved = true,
                    IsLocked = false,
                    PasswordFailureCount = 0,
                    Role = roles.Where(x => x.Type == 1).FirstOrDefault(),
                   Orgin=orgins.Where(x => x.OrginName=="太陽能組").FirstOrDefault()
                }             
            };
            return accounts;
        }

        private List<Bulletin> InitializeBulletin(List<Orgin> orgins)
        {
            List<Bulletin> bulletins = new List<Bulletin>
            {
                new Bulletin()
                {
                    title = "豪雨",
                    context = "水利發電創新高，百年不間斷發電",
                    CreateDate = DateTime.Now,
                    Orgin = orgins.Where(x => x.OrginCode==0).FirstOrDefault()
                },
                new Bulletin()
                {
                    title = "獵日",
                    context = "Solar發電創新高，24H不間斷發電",
                    CreateDate = DateTime.Now,
                    Orgin = orgins.Where(x => x.OrginCode==0).FirstOrDefault()
                },
                new Bulletin()
                {
                    title = "狂風",
                    context = "風機發電創新高，24H萬轉不間斷",
                    CreateDate = DateTime.Now,
                    Orgin = orgins.Where(x => x.OrginCode==0).FirstOrDefault()
                },
                new Bulletin()
                {
                    title = "月蝕",
                    context = "Solar發電創新高，夜間發電不間斷",
                    CreateDate = DateTime.Now,
                    Orgin = orgins.Where(x => x.OrginCode==0).FirstOrDefault()
                },

                new Bulletin()
                {
                    title = "核爆",
                    context = "核融合發電創新高，萬年發電不滅",
                    CreateDate = DateTime.Now,
                    Orgin = orgins.Where(x => x.OrginCode==0).FirstOrDefault()
                },
                new Bulletin()
                {
                    title = "十級小地震",
                    context = "發電創新低，不發電",
                    CreateDate = DateTime.Now,
                    Orgin = orgins.Where(x => x.OrginCode==0).FirstOrDefault()
                },
            };
            return bulletins;
        }
        private List<AlartType> InitializeAlartType()
        {
            List<AlartType> alartTypes = new List<AlartType>
            {
                new AlartType()
                {
                    AlartTypeName = "所有異常"
                },
                new AlartType()
                {
                    AlartTypeName = "市電"
                },
                new AlartType()
                {
                    AlartTypeName = "電池"
                },
                new AlartType()
                {
                    AlartTypeName = "太陽能"
                },
                new AlartType()
                {
                    AlartTypeName = "負載"
                },
                new AlartType()
                {
                    AlartTypeName = "發電機"
                },
                new AlartType()
                {
                    AlartTypeName = "逆變器"
                }
            };
            return alartTypes;
        }

        private List<Station> InitializeStation()
        {
            List<Station> station = new List<Station>
            {
                new Station()
                {
                    StationName = "所有站別"
                },
                new Station()
                {
                    StationName = "林邊鄉光彩濕地"
                },
                new Station()
                {
                    StationName = "霧台鄉大武社區活動中心"
                },
                                new Station()
                {
                    StationName = "泰武鄉佳興社區活動中心"
                }
            };
            return station;
        }

        private List<Alart> InitializeAlart(List<AlartType> alartTypes, List<Station> stations)
        {
            List<Alart> alart = new List<Alart>
            {
                new Alart()
                {
                    AlartType = alartTypes.Where(x => x.AlartTypeName=="所有異常").FirstOrDefault(),
                    Station = stations.Where(x => x.StationName=="所有站別").FirstOrDefault(),
                    AlartContext = "AAAA",
                    StartTimet = DateTime.Now,
                    EndTimet = DateTime.Now,
                    Disabled = false
        },
                new Alart()
                {
                    AlartType = alartTypes.Where(x => x.AlartTypeName=="行控中心").FirstOrDefault(),
                    Station = stations.Where(x => x.StationName=="林邊").FirstOrDefault(),
                    AlartContext = "BBB",
                    StartTimet = DateTime.Now,
                    EndTimet = DateTime.Now,
                    Disabled = false

                },
                new Alart()
                {
                    AlartType = alartTypes.Where(x => x.AlartTypeName=="市電").FirstOrDefault(),
                    Station = stations.Where(x => x.StationName=="墾丁").FirstOrDefault(),
                    AlartContext = "CCC",
                    StartTimet = DateTime.Now,
                    EndTimet = DateTime.Now,
                    Disabled = false
                }
            };
            return alart;
        }

        #endregion  Initialize

    }
}
