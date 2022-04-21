using System.Collections.Generic;
using System.Linq;

namespace AdfsAuthenticationHandler.DI.Models
{
    public class RoleCheckerConfig
    {
        public List<ActionItem> Items { get; set; } = new List<ActionItem>();

        public ActionItemConfig FallbackConfig { get; set; } = ActionItemConfig.MakeForUnMappedDoNotAllow();

        public string ApiPrefix { get; set; } = "/api";

        public void SetFallbackConfig(string fallbackRole)
        {
            var config = ActionItemConfig.MakeForRole(fallbackRole);
            SetFallbackConfig(config);
        }

        public void SetFallbackConfig(ActionItemConfig fallback)
        {
            FallbackConfig = fallback;
        }

        public void AddAnonItem(string controller, string actionName, bool hasParamBeforeAction = false)
        {
            AddItem(controller, actionName, ActionItemConfig.MakeForAllowAnon(), hasParamBeforeAction);
        }

        public ActionItemConfig AddItem(string controller, string actionName, string role, bool hasParamBeforeAction = false)
        {
            return AddItem(controller, actionName, new string[] { role }, hasParamBeforeAction);
        }

        public ActionItemConfig AddItem(string controller, string actionName, string[] roles, bool hasParamBeforeAction = false)
        {
            ActionItemConfig config = ActionItemConfig.MakeForRoles(roles);
            AddItem(controller, actionName, config, hasParamBeforeAction);
            return config;
        }


        public ActionItemConfig AddItem(string controller, string actionName, ActionItemConfig config, bool hasParamBeforeAction)
        {
            var existing = Items
                .Where(i => i.ControllerName.ToLower() == controller.ToLower())
                .Where(i => i.ActionName.ToLower() == actionName.ToLower())
                .Any();

            if (existing)
            {
                throw new System.Exception("DUPLICATE ADD");
            }

            AddActionItem(controller, actionName, config, hasParamBeforeAction);
            return config;
        }

        private ActionItemConfig AddActionItem(string controller, string actionName, ActionItemConfig config, bool hasParamBeforeAction)
        {
            ActionItem newItem = ActionItem.Make(controller, actionName, hasParamBeforeAction, config);
            Items.Add(newItem);

            return config;
        }

        public ActionItemConfig GetRolesForPath(string controllerName, string actionName, string path)
        {
            //try to get config by controller name and action name
            var foundItem = Items.Where(i => string.IsNullOrWhiteSpace(controllerName) == false && i.ControllerName.ToLower() == controllerName.ToLower())
                                 .Where(i => string.IsNullOrWhiteSpace(actionName) == false && i.ActionName.ToLower() == actionName.ToLower())
                                 .FirstOrDefault();
            if(foundItem != null)
            {
                return foundItem.Config;
            }


            //No try to get config by parsing the path
            var item = Items.FirstOrDefault(i => i.ToPath(ApiPrefix).ToLower() == path.ToLower());

            if (item != null)
            {
                return item.Config;
            }

            if (string.IsNullOrWhiteSpace(ApiPrefix) == false)
            {
                path = path.Replace($"{ApiPrefix}/", string.Empty);
            }

            var parts = path.Split('/').Where(p => string.IsNullOrWhiteSpace(p) == false).ToArray();

            string controller = parts[0];
            string action = "#ANON_CONTROLLER_ACTION#";
            if (parts.Length >= 2)
            {
                action = parts[1];
            }

            var withoutParams = Items
                .Where(i => i.ControllerName.ToLower() == controller.ToLower())
                .Where(i => i.ActionName.ToLower() == action.ToLower())
                .FirstOrDefault();

            if (withoutParams != null)
            {
                return withoutParams.Config;
            }

            var wildCards = Items
                .Where(i => i.ControllerName.ToLower() == controller.ToLower())
                .Where(i => i.ActionName == "*")
                .FirstOrDefault();

            if (wildCards != null)
            {
                return wildCards.Config;
            }

            if (parts.Length == 3)
            {
                var parameter = parts[1];
                action = parts[2];

                var withParams = Items
                .Where(i => i.ControllerName.ToLower() == controller.ToLower())
                .Where(i => i.ActionName.ToLower() == $"*/{action}")
                .Where(i => i.HasParameterBeforeAction)
                .FirstOrDefault();

                if (withParams != null)
                {
                    return withParams.Config;
                }

            }

            return FallbackConfig;
        }
    }

    public class ActionItem
    {
        public string ControllerName { get; }

        public string ActionName { get; }

        public bool HasParameterBeforeAction { get; } = false;

        public ActionItemConfig Config { get; }

        ActionItem(string controllerName, string actionName, bool hasParameterBeforeAction, ActionItemConfig config)
        {
            ControllerName = controllerName;
            ActionName = actionName;
            HasParameterBeforeAction = hasParameterBeforeAction;
            Config = config;
        }

        public static ActionItem Make(string controllerName, string actionName, bool hasParameterBeforeAction, ActionItemConfig config)
        {
            return new ActionItem(controllerName, actionName, hasParameterBeforeAction, config);
        }

        public string ToPath(string prefix = null)
        {
            if (string.IsNullOrWhiteSpace(prefix))
            {
                return $"/{ControllerName}/{ActionName}";
            }
            return $"{prefix}/{ControllerName}/{ActionName}";
        }
    }

    public class ActionItemConfig
    {
        public bool AllowAnon { get; } = false;

        public string[] Roles { get; } = new string[0];

        public bool BypassUserTokenCheck { get; private set; } = false;

        ActionItemConfig(bool allowAnon, string[] roles)
        {
            AllowAnon = allowAnon;
            Roles = roles;
        }

        public ActionItemConfig AllowUserCheckBypass(bool bypass = true)
        {
            BypassUserTokenCheck = bypass;

            return this;
        }

        static ActionItemConfig Make(bool allowAnon, string[] roles)
        {
            return new ActionItemConfig(allowAnon, roles);
        }

        public static ActionItemConfig MakeForRoles(string[] roles)
        {
            return Make(false, roles);
        }

        public static ActionItemConfig MakeForRole(string role)
        {
            return Make(false, new string[] { role });
        }

        public static ActionItemConfig MakeForAllowAnon()
        {
            return Make(true, new string[0]);
        }

        public static ActionItemConfig MakeForUnMappedDoNotAllow()
        {
            return Make(false, new string[0]);
        }
    }
}
