using System;
using System.Collections.Generic;
using System.Linq;

namespace STR_Localizacion.UTIL
{
    public class sapitemevent
    {
        public delegate void saphandler(SAPbouiCOM.ItemEvent e);

        private string[] _controlname;
        private SAPbouiCOM.BoEventTypes _eventtype;
        private saphandler _handler;

        public string[] controlname { get { return _controlname; } }
        public saphandler handler { get { return _handler; } }
        public SAPbouiCOM.BoEventTypes eventtype { get { return _eventtype; } }

        public sapitemevent(SAPbouiCOM.BoEventTypes eventtype, string controlname, saphandler handler)
        {
            _eventtype = eventtype;
            _controlname = new string[] { controlname };
            _handler = handler;
        }

        public sapitemevent(string controlname, saphandler handler)
        {
            _controlname = new string[] { controlname };
            _handler = handler;
        }

        public sapitemevent(SAPbouiCOM.BoEventTypes eventtype, saphandler handler, params string[] controlname)
        {
            _eventtype = eventtype;
            _controlname = controlname;
            _handler = handler;
        }

        public sapitemevent(saphandler handler, params string[] controlname)
        {
            _controlname = controlname;
            _handler = handler;
        }

        public void setEventType(SAPbouiCOM.BoEventTypes etype)
        {
            _eventtype = etype;
        }
    }

    public class sapmenuevent
    {
        public delegate void saphandler(SAPbouiCOM.MenuEvent e);

        private string[] _menuid;
        private saphandler _handler;

        public string[] menuid { get { return _menuid; } }
        public saphandler handler { get { return _handler; } }

        public sapmenuevent(string menuid, saphandler handler)
        {
            _menuid = new string[] { menuid };
            _handler = handler;
        }

        public sapmenuevent(saphandler handler, params string[] menuid)
        {
            _menuid = menuid;
            _handler = handler;
        }
    }

    public class sapmenueventList : List<sapmenuevent>
    {
        public bool Perform(SAPbouiCOM.MenuEvent menuevent)
        {
            bool bubbleevent = true;
            sapmenuevent process;
            try
            {
                if (menuevent == null)
                    return bubbleevent;
                else
                    process = this.FirstOrDefault(s => s.menuid.Any(e => e.Equals(menuevent.MenuUID)));

                if (process != null)
                    process.handler(menuevent);
            }
            catch (Exception ex)
            {
                string tipo = string.Empty;

                if (ex is ArgumentException)
                    tipo = "faltan argumentos ";
                else if (ex is InvalidOperationException)
                    tipo = "operación inválida";
                else
                    tipo = "error";

                Cls_Global.go_SBOApplication.StatusBar.
                    SetText(string.Format("MenuEvent {0}: {1}", tipo, ex.Message), SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);

                bubbleevent = false;
            }

            return bubbleevent;
        }
    }

    public class sapitemeventList : List<sapitemevent>
    {
        public new void Add(sapitemevent item)
        {
            if (item.eventtype != null)
                base.Add(item);
            else
                throw new ArgumentException("El tipo de evento debe ser especificado", "eventtype");
        }

        public void Add(SAPbouiCOM.BoEventTypes eventtype, params sapitemevent[] items)
        {
            for (int i = 0; i < items.Count(); i++)
                items.ElementAt(i).setEventType(eventtype);

            AddRange(items);
        }

        public bool Perform(SAPbouiCOM.ItemEvent itemevent)
        {
            bool bubbleevent = true;
            sapitemevent process;
            try
            {
                if (itemevent == null)
                    return bubbleevent;
                else if (itemevent.EventType == SAPbouiCOM.BoEventTypes.et_FORM_UNLOAD ||
                         itemevent.EventType == SAPbouiCOM.BoEventTypes.et_FORM_LOAD ||
                         itemevent.EventType == SAPbouiCOM.BoEventTypes.et_FORM_ACTIVATE ||
                         itemevent.EventType == SAPbouiCOM.BoEventTypes.et_FORM_RESIZE)
                    process = this.FirstOrDefault(s => s.eventtype.Equals(itemevent.EventType));
                else if (string.IsNullOrEmpty(itemevent.ItemUID))
                    return bubbleevent;
                else
                    process = this.FirstOrDefault(s => s.eventtype.Equals(itemevent.EventType) &&
                        s.controlname.Any(e => e.Equals(itemevent.ItemUID)));

                if (process != null)
                    process.handler(itemevent);
            }
            catch (Exception ex)
            {
                string tipo = string.Empty;

                if (ex is ArgumentException)
                    tipo = "faltan argumentos ";
                else if (ex is InvalidOperationException)
                    tipo = "operación inválida";
                else
                    tipo = "error";

                Cls_Global.go_SBOApplication.StatusBar.
                    SetText(string.Format("ItemEvent {0}: {1}", tipo, ex.Message), SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);

                bubbleevent = false;
            }

            return bubbleevent;
        }
    }

    public class internalexception
    {
        public enum TCatch
        {
            Exito = 0,
            Error = 1,
            Excepcion = 2,
            Info = 3,
        }

        private string _class;
        private string _method;
        private string _layout;
        private TCatch _type = TCatch.Excepcion;
        private string _message;

        public string NameLayout { get { return _layout; } }
        public string NameClass { get { return _class; } }

        public string NameMethod
        {
            get { return _method; }
            //set { if (value != _method) _method = value; }
        }

        public SAPbouiCOM.BoStatusBarMessageType MessageType
        {
            get
            {
                switch (_type)
                {
                    case TCatch.Info:
                        return SAPbouiCOM.BoStatusBarMessageType.smt_Warning;

                    case TCatch.Exito:
                        return SAPbouiCOM.BoStatusBarMessageType.smt_Success;

                    case TCatch.Excepcion:
                        return SAPbouiCOM.BoStatusBarMessageType.smt_Warning;

                    case TCatch.Error:
                    default:
                        return SAPbouiCOM.BoStatusBarMessageType.smt_Error;
                }
            }
        }

        public string Message
        {
            get { return _message; }
            //set { if (value != _message) _message = value; }
        }

        public string Exception
        {
            get
            {
                if (string.IsNullOrEmpty(_method))
                    throw new ArgumentNullException("NameMethod");
                else if (string.IsNullOrEmpty(_message))
                    throw new ArgumentNullException("Message");

                string m_res = string.Empty;
                switch (_type)
                {
                    case TCatch.Info:
                        m_res = string.Format("{0}", Message);
                        break;

                    case TCatch.Exito:
                        m_res = string.Format("OK: {0}", Message);
                        break;

                    case TCatch.Excepcion:
                        m_res = string.Format("EX: {0}", Message);
                        break;

                    case TCatch.Error:
                        m_res = string.Format("ER: {0} - POINT ({1}_{2}_{3})", Message, NameMethod, NameClass, NameLayout);
                        break;
                }

                return m_res;
            }
        }

        public internalexception(string classname, string layoutname)
        {
            _class = classname;
            _layout = layoutname;
        }

        public void inner(string message, string methodname)
        {
            _message = message;
            _method = methodname;
        }

        public void inner(TCatch tcatch, string message, string methodname)
        {
            _type = tcatch;
            _message = message;
            _method = methodname;
        }
    }
}