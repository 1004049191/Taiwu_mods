using Harmony12;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityModManagerNet;

namespace CharacterFloatInfo
{
    public class SpecialSocial
    {
        public string getStr()
        {
            return "���Գɹ�";
        }
        List<string> socialWords = new List<string> { };

        public string getActorName(int id, bool realName = false, bool baseName = false)
        {
            return DateFile.instance.GetActorName(id, realName, baseName);
        }

        //��������֯�еȼ�ID
        public static int GetGangLevelId(int id)
        {
            int num2 = int.Parse(DateFile.instance.GetActorDate(id, 19, false));
            int num3 = int.Parse(DateFile.instance.GetActorDate(id, 20, false));
            int gangValueId = DateFile.instance.GetGangValueId(num2, num3);
            return gangValueId;
        }

        //��ȡ��ĸ������֯�ȼ�
        public string getParentGangLevel(int actorId)
        {
            return "����";
        }
        //��ȡ��ĸ������
        public string getParentAlive(int actorId)
        {
            List<int> list = new List<int>(DateFile.instance.GetActorSocial(actorId, 303, true));//�����ѹ���
            List<int> list2 = new List<int>(DateFile.instance.GetActorSocial(actorId, 303, false));
            int count = list.Count;
            int count2 = list2.Count;
            switch (count)
            {
                case 0:
                    return "�޸���ĸ";
                    break;
                case 1:
                    return "��֪����";
                    break;
                case 2:
                    switch (count2)
                    {
                        case 0:
                            return "��ĸ˫��";
                            break;
                        case 1:
                            return "�¸���ĸ";
                            break;
                        case 2:
                            return "˫�׽���";
                            break;
                    }
                    break;
                case 3:
                    return "��ĸ�ٻ�";
                    break;
            }
            return "";
        }


        //TODO.������У�����ϵ�������룬�����ö�Ӧ��ɫƷ��

        //������ĸ���
        public string analyzeParent(int actorId)
        {
            string text = "";
            List<int> list = new List<int>(DateFile.instance.GetActorSocial(actorId, 303, true));
            int count = list.Count;

            //text += getParentAlive(actorId);
            socialWords.Add(getParentAlive(actorId));

            for (int i = 0; i < count; i++)
            {
                int nid = list[i];
                int gangLv = GetGangLevelId(nid);
                if (gangLv % 10 <= 3)
                {
                    socialWords.Add("�߸��ӵ�"); 
                    break;
                }
            }            
            return string.Join(", ", socialWords.ToArray());
        }


        public void testAll(int actorId)
        {
            DateFile df = DateFile.instance;
            //DateFile.instance.GetActorSocial(id, 310, false).Count
            Main.Logger.Log("---------------------------------------------------");
            for (int i = 0; i < 12; i++)
            {
                int typ = 301 + i;
                if (true)
                {
                    if (df.HaveLifeDate(actorId, typ))
                    {
                        List<int> list = new List<int>(df.GetActorSocial(actorId, typ, true));
                        for (int j = 0; j < list.Count; j++)
                        {
                            int aId = list[j];
                            Main.Logger.Log(string.Format("ShowAcotrSocial:Index:{0},Key:{1},Value:{2},Name:{3}", typ, j, aId, this.getActorName(aId)));
                        }
                    }
                }
            }
        }

        //301Ī��֮�� 302�ֵܽ��� 303������ĸ 304�常��ĸ 305��ҵ��ʦ 306��������
        //307�������� 308������ 309��ż 310���� 312���İ�Ľ ��311���Ʋ�Ϊ��ϵ����
        public string analyzeSocial(int actorId)
        {
            DateFile df = DateFile.instance;
            //DateFile.instance.GetActorSocial(id, 310, false).Count
            Main.Logger.Log("---------------------------------------------------");
            return this.analyzeParent(actorId);
        }
    }
}