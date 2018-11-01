using Harmony12;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;
using UnityModManagerNet;

namespace TaiwuBaseClass
{
    public class Actor
    {
        public int Id;


        public struct PersonName
        {
            private int id;
            public string First
            {
                get
                {
                    return DateFile.instance.GetActorDate(id, 29, false);
                }
            }

            public string Last
            {
                get
                {
                    return DateFile.instance.GetActorDate(id, 2, false);
                }
            }

            public PersonName(int id)
            {
                this.id = id;
            }
        }
        private PersonName _name;

        public PersonName Name
        {
            get
            {
                return _name;
            }
        }


        public int Age//����
        {
            get
            {
                return int.Parse(DateFile.instance.GetActorDate(Id, 11, false));
            }
        }
        public int Health//����
        {
            get
            {
                return ActorMenu.instance.Health(Id);
            }

        }
        public int Str//����
        {
            get
            {
                return DateFile.instance.BaseAttr(Id, 0, 0);
            }
        }
        public int Con//����
        {
            get
            {
                return DateFile.instance.BaseAttr(Id, 1, 0);
            }
        }
        public int Agi//����
        {
            get
            {
                return DateFile.instance.BaseAttr(Id, 2, 0);
            }
        }
        public int Bon//����
        {
            get
            {
                return DateFile.instance.BaseAttr(Id, 3, 0);
            }
        }
        public int Int//����
        {
            get
            {
                return DateFile.instance.BaseAttr(Id, 4, 0);
            }
        }
        public int Pat//����
        {
            get
            {
                return DateFile.instance.BaseAttr(Id, 5, 0);
            }
        }

        public int Gender//�Ա�
        {
            get
            {
                return int.Parse(DateFile.instance.GetActorDate(Id, 14, false));
            }
        }
        public int Charm//����
        {
            get
            {
                return int.Parse(DateFile.instance.GetActorDate(Id, 15, false));
            }
        }
        public struct Samsara
        {
            public int Count;
            public List<int> SamsaraList;
            public List<string> samNameList;

            public Samsara(int id)
            {
                SamsaraList = DateFile.instance.GetLifeDateList(id, 801, false);
                Count = SamsaraList.Count;
                samNameList = new List<string>();
                foreach (int i in SamsaraList)
                {
                    samNameList.Add(DateFile.instance.GetActorName(i));
                }
            }
        }

        public struct PersonKungfu//��ѧ����
        {
            private int id;

            public int Force//�ڹ�
            {
                get
                {
                    return int.Parse(DateFile.instance.GetActorDate(id, 601, true));
                }
            }
            public int Dodge//��
            {
                get
                {
                    return int.Parse(DateFile.instance.GetActorDate(id, 602, true));
                }
            }
            public int Stunt//����
            {
                get
                {
                    return int.Parse(DateFile.instance.GetActorDate(id, 603, true));
                }
            }
            public int Strike//�Ʒ�
            {
                get
                {
                    return int.Parse(DateFile.instance.GetActorDate(id, 604, true));
                }
            }
            public int Finger//ָ��
            {
                get
                {
                    return int.Parse(DateFile.instance.GetActorDate(id, 605, true));
                }
            }
            public int Kick//�ȷ�
            {
                get
                {
                    return int.Parse(DateFile.instance.GetActorDate(id, 606, true));
                }
            }
            public int HidWeapon//����
            {
                get
                {
                    return int.Parse(DateFile.instance.GetActorDate(id, 607, true));
                }
            }
            public int Sword//����
            {
                get
                {
                    return int.Parse(DateFile.instance.GetActorDate(id, 608, true));
                }
            }
            public int Blade//����
            {
                get
                {
                    return int.Parse(DateFile.instance.GetActorDate(id, 609, true));
                }
            }
            public int Longstick//����
            {
                get
                {
                    return int.Parse(DateFile.instance.GetActorDate(id, 610, true));
                }
            }
            public int Qimen//����
            {
                get
                {
                    return int.Parse(DateFile.instance.GetActorDate(id, 611, true));
                }
            }
            public int Whip//���
            {
                get
                {
                    return int.Parse(DateFile.instance.GetActorDate(id, 612, true));
                }
            }
            public int Shoot//����
            {
                get
                {
                    return int.Parse(DateFile.instance.GetActorDate(id, 613, true));
                }
            }
            public int Instrument//����
            {
                get
                {
                    return int.Parse(DateFile.instance.GetActorDate(id, 614, true));
                }
            }

            public PersonKungfu(int id)
            {
                this.id = id;
            }
        }

        private PersonKungfu _kungfu;

        public PersonKungfu Kungfu//��ѧ����
        {
            get
            {
                return _kungfu;

            }
        }

        public struct PersonSkill//��������
        {
            private int id;

            public int Music//����
            {
                get
                {
                    return int.Parse(DateFile.instance.GetActorDate(id, 501, true));
                }
            }
            public int ChessArt//����
            {
                get
                {
                    return int.Parse(DateFile.instance.GetActorDate(id, 502, true));
                }
            }
            public int Poem//ʫ��
            {
                get
                {
                    return int.Parse(DateFile.instance.GetActorDate(id, 503, true));
                }
            }
            public int Paint//�滭
            {
                get
                {
                    return int.Parse(DateFile.instance.GetActorDate(id, 504, true));
                }
            }
            public int Math//����
            {
                get
                {
                    return int.Parse(DateFile.instance.GetActorDate(id, 505, true));
                }
            }
            public int Tasting//Ʒ��
            {
                get
                {
                    return int.Parse(DateFile.instance.GetActorDate(id, 506, true));
                }
            }
            public int Smith//����
            {
                get
                {
                    return int.Parse(DateFile.instance.GetActorDate(id, 507, true));
                }
            }
            public int Wood//��ľ
            {
                get
                {
                    return int.Parse(DateFile.instance.GetActorDate(id, 508, true));
                }
            }
            public int Medical//ҽ��
            {
                get
                {
                    return int.Parse(DateFile.instance.GetActorDate(id, 509, true));
                }
            }
            public int Poison//����
            {
                get
                {
                    return int.Parse(DateFile.instance.GetActorDate(id, 510, true));
                }
            }
            public int Cloth//֯��
            {
                get
                {
                    return int.Parse(DateFile.instance.GetActorDate(id, 511, true));
                }
            }
            public int Jwelry//�ɽ�
            {
                get
                {
                    return int.Parse(DateFile.instance.GetActorDate(id, 512, true));
                }
            }
            public int Taoism//����
            {
                get
                {
                    return int.Parse(DateFile.instance.GetActorDate(id, 513, true));
                }
            }
            public int Buddhism//��ѧ
            {
                get
                {
                    return int.Parse(DateFile.instance.GetActorDate(id, 514, true));
                }
            }
            public int Cooking//����
            {
                get
                {
                    return int.Parse(DateFile.instance.GetActorDate(id, 515, true));
                }
            }
            public int MisArt//��ѧ
            {
                get
                {
                    return int.Parse(DateFile.instance.GetActorDate(id, 516, true));
                }
            }


            public PersonSkill(int id)
            {
                this.id = id;
            }
        }

        private PersonSkill _skill;

        public PersonSkill Skill
        {
            get
            {
                return _skill;
            }
        }

        public struct PersonStatus//����״̬
        {
            private int id;
            public int MaxHp//��������
            {
                get
                {
                    return ActorMenu.instance.MaxHp(id);
                }
            }
            public int Hp//����
            {
                get
                {
                    return ActorMenu.instance.Hp(id, false);
                }
            }
            public int MaxSp//��������
            {
                get
                {
                    return ActorMenu.instance.MaxSp(id);
                }
            }
            public int Sp//����
            {
                get
                {
                    return ActorMenu.instance.Sp(id, false);
                }
            }
            public List<int> ResPoisons//���ؿ���
            {
                get
                {
                    List<int> list = new List<int> { };
                    for (int i = 0; i < 6; i++)
                    {
                        int num = int.Parse(DateFile.instance.GetActorDate(id, 42 + i, false));
                        list.Add(num);
                    }
                    return list;
                }
            }
            public List<int> Poisons//�ж�
            {
                get
                {
                    List<int> list = new List<int> { };
                    for (int i = 0; i < 6; i++)
                    {
                        int num = int.Parse(DateFile.instance.GetActorDate(id, 51 + i, false));
                        list.Add(num);
                    }
                    return list;
                }
            }


            public PersonStatus(int id)
            {
                this.id = id;
            }
        }

        private PersonStatus _status;
        public PersonStatus Stauts
        {
            get
            {
                return _status;
            }
        }

        protected Actor()
        {

        }

        public static Actor fromId(int id)
        {
            if (!DateFile.instance.actorsDate.ContainsKey(id))
            {
                return null;
            }
            var actor = new Actor();
            actor.Id = id;
            return actor;
        }
    }

    public class ActorGroup:Actor
    {
        private int id;
        public int GroupId
        {
            get
            {
                return int.Parse(DateFile.instance.GetActorDate(id, 19, false));
            }
        }
        public string GroupName
        {
            get
            {
                return DateFile.instance.GetGangDate(GroupId, 0);
            }
        }
        public int GroupLvl
        {
            get
            {
                int num1 = int.Parse(DateFile.instance.GetActorDate(id, 19, false));
                int num2 = int.Parse(DateFile.instance.GetActorDate(id, 20, false));
                int gangValueId = DateFile.instance.GetGangValueId(num1, num2);
                return gangValueId;
            }
        }
        public string GroupLvlName
        {
            get
            {
                int num = int.Parse(DateFile.instance.GetActorDate(id, 20, false));
                int key = (num >= 0) ? 1001 : (1001 + int.Parse(DateFile.instance.GetActorDate(id, 14, false)));
                string gang = DateFile.instance.presetGangGroupDateValue[GroupLvl][key];
                return gang;
            }
        }
        
        public ActorGroup(int id)
        {
            this.id = id;
        }
    }
}