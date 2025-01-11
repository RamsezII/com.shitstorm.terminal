//using System.Collections.Generic;
//using System.Linq;
//using TMPro;
//using UnityEngine;

//namespace _TERMINAL_
//{
//    public partial class CommandLineUI : MonoBehaviour
//    {
//        public static CommandLineUI instance;

//        [SerializeField] TMP_Dropdown prefab_dropdown;

//        readonly List<TMP_Dropdown> dropdowns = new();

//        //------------------------------------------------------------------------------------------------------------

//        private void Awake()
//        {
//            instance = this;
//            prefab_dropdown = transform.Find("Layout/Dropdown").GetComponent<TMP_Dropdown>();
//        }

//        //------------------------------------------------------------------------------------------------------------

//        private void Start()
//        {
//            prefab_dropdown.gameObject.SetActive(false);

//            TMP_Dropdown dropdown = Instantiate(prefab_dropdown, prefab_dropdown.transform.parent);
//            dropdown.gameObject.SetActive(true);

//            dropdowns.Add(dropdown);

//            Shell.commands.AddListener(value =>
//            {
//                dropdown.ClearOptions();
//                dropdown.AddOptions(value.ToList());
//            });

//            dropdown.onValueChanged.AddListener(OnDropDownValue);
//        }

//        //------------------------------------------------------------------------------------------------------------

//        void OnDropDownValue(int index)
//        {

//        }

//        //------------------------------------------------------------------------------------------------------------

//        private void OnDestroy()
//        {
//            if (this == instance)
//                instance = null;
//        }
//    }
//}