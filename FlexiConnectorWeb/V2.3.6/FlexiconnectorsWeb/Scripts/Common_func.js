
// Javascript log4j root functionality //
var js_filename = "";
js_filename = "Common_func.js";

function getCurentForm_FileName() {
    var pagePathName = window.location.pathname;
    return pagePathName.substring(pagePathName.lastIndexOf("/") + 1);
}

function javascript_log4j_root(methodName, errmsg) {
    try {
        var err_data = {};
        err_data.formName = getCurentForm_FileName();
        err_data.filename = js_filename;
        err_data.methodName = methodName.toString();
        err_data.errmsg = errmsg.toString();
        var result = ajaxcall_param('/Home/javascript_log4j', JSON.stringify(err_data));
    }
    catch (err) {
        alert(err);
    }
}


// Session Timeout Function //
var interval;
var serverExit;
interval = window.setInterval('sessioncheck()', 60 * 1000 * 60);

//pageload_fn();

function pageload_fn() {
    timeinterval_reset();
    if ($("#lblloginTimeMsg").val() != undefined)
        $("#lblloginTimeMsg").text("You are logged in Time is : " + getlocalStorage('crtloginTime'));
}

function timeinterval_reset() {
    clearInterval(interval);
    interval = 0;
    interval = window.setInterval('sessioncheck()', 60 * 1000 * 60);
}

function webSession_Exit() {
    //pageload_fn();
    serverExit = $('#txtserverExit').val();
    if (serverExit == "true") {
        sessioncheck();
    }
}

function sessioncheck() {
    kendo_Exit_Alert("Your session has expired please relaunch the application.");
}

function sec_Permission(ScreenId) {
    $("#btn_new").attr("disabled", true);   
    $("#btn_Edit").attr("disabled", true);
    $("#btn_View").attr("disabled", true);
    $("#divEdit #deactivate").prop("disabled", true);   
    $("#btnApprove").css("display", "none");
    $("#btnReject").css("display", "none");
    var permition = getlocalStorage('sec_permission');
    var obj = [];
    var perm = {};
    perm.new = false;
    perm.modify = false;
    perm.delete = false;
    perm.view = false;
    perm.deny = false;
    perm.print = false;
    perm.auth = false;
    $.each(permition, function (key, rec) {
        if (rec.ScreenId == ScreenId) {
            obj.push(rec);
            if (obj[0].AddPerm == "1") {
                $("#btn_new").attr("disabled", false);
                perm.new = true;
            }
            if (obj[0].AuthPerm == "1") {
                $('#divEdit #reviewEd').hide();
                $("#divEdit #deactivate").hide();
                $("#btnSaveEd").hide();
                $("#btnApprove").show();
                $("#btnReject").show();
                $("#btn_Edit").attr("disabled", false);
                perm.auth = true;
            }

            if (obj[0].ModPerm == "1") {
                $("#btn_Edit").attr("disabled", false);
                perm.modify = true;
            }
            if (obj[0].InactPerm == "1") {
                $("#divEdit #deactivate").attr("disabled", false);
                perm.delete = true;
            }
            if (obj[0].ViewPerm == "1" || obj[0].ViewPerm == "0") {
                $("#btn_View").attr("disabled", false);
                perm.view = true;
            }
           
            if (obj[0].PrnPerm == "1") {
                perm.print = true;
            }

        }
    });
    return perm;
}


function sec_Master_Permission(ScreenId) {
    $("#btnRnew").prop("disabled", true);   
    $("#btnRSave").prop("disabled", true);
    $("#inactivate").prop("disabled", true);
    var permition = getlocalStorage('sec_permission');
    var obj = [];
    var perm = {};
    perm.new = false;
    perm.modify = false;
    perm.delete = false;
    perm.view = false;
    perm.deny = false;
    perm.print = false;
    $.each(permition, function (key, rec) {
        if (rec.ScreenId == ScreenId) {
            obj.push(rec);
            if (obj[0].AddPerm == "1") {
                $("#btnRnew").prop("disabled", false);
                $("#btnRSave").prop("disabled", false);
                perm.new = true;
            }
            if (obj[0].ModPerm == "1") {
                $("#btnRnew").prop("disabled", false);
                $("#btnRSave").prop("disabled", false);
                perm.modify = true;
            }
            if (obj[0].InactPerm == "1") {
                $("#inactivate").prop("disabled",false);
                perm.delete = true;
            }
            if (obj[0].ViewPerm == "1") {
                $("#btnRnew").prop("disabled", true);
                $("#btnRSave").prop("disabled", true);
                $("#inactivate").prop("disabled", true);
                perm.view = true;
            }
            if (obj[0].PrnPerm == "1") {
                perm.print = true;
            }
        }
    });
    return perm;
}



var fltID = "";
var docno = "";
var doctype = "";
var scrnid = "";
var hdrtitle = "";
var searchid = ""; // help 
var cmbMasterId = "";
var cmbMasterDesc = "";

var detail2 = "";
var detail1 = "";

var profile = "";

var click_ctrl_name = "";
var click_act_name = "";
var form_list_url = "";
var hdnCode = "";
var hdnGrid = "";
var Org_screen_id = "";
var Org_grid_id = "";
var list_grid_id = "";
var Main_grid = "";
var Excel_perm = "";
var Screen_Id = "";
var Grid_Id = "";
var org_menu_id = "";
var index_hid = {};
var field_hid = {};
var TreeId = "";
var SubTreeId = "";

var FormName = "";
var Form_validate_Name = "";
var detail_obj = {};
var hdrtitleGrant = "";
var Global_row_id = "";

function setlocalStorage(key, value) {
    var guid = sessionStorage.getItem('gui_value');
    var data = JSON.parse(localStorage[guid]);
    data[key] = value;
    localStorage.setItem(guid, JSON.stringify(data));
}

function getlocalStorage(key) {
    var guid = sessionStorage.getItem('gui_value');
    var data = JSON.parse(localStorage[guid]);
    return data[key];
}

function creatlocalStorage(guid) {
    var data = {};
    localStorage.setItem(guid, JSON.stringify(data));
    sessionStorage.setItem('gui_value', guid);
}


function removelocalStorage(key) {
    var guid = sessionStorage.getItem('gui_value');
    var localdata = JSON.parse(localStorage[guid]);
    delete localdata[key];
    localStorage.setItem(guid, JSON.stringify(localdata));
}

$(function () {
    $('a.adv_filter').click(function () {
        try {
            fltID = $(this).attr('fltID');
            hdrtitle = $(this).attr('hdrtitle');
            if (fltID == undefined || fltID == "") {
                kendoAlert("Check below attributes are mandatory fltID");
                return false;
            }
        }
        catch (err) {
            alert(err);
        }

    });


    $('a.OrgView').click(function () {
        try {
            hdrtitle = $(this).attr('hdrtitle');
        }
        catch (err) {
            alert(err);
        }
    });

    $('a.OrgView').click(function () {
        try {
            Org_screen_id = $(this).attr('Org_screen_id');
            Org_grid_id = $(this).attr('Org_grid_id');

        }
        catch (err) {
            alert(err);
        }

    });


    $('a.crt_task').click(function () {
        try {
            hdrtitle = $(this).attr('hdrtitle');
            scrnid = $(this).attr('scrnid');
            detail1 = $(this).attr('detail1');
            detail2 = $(this).attr('detail2');
        }
        catch (err) {
            alert(err);
        }
    });


    $('a.chklist').click(function () {
        try {
            hdrtitle = $(this).attr('hdrtitle');

        }
        catch (err) {
            alert(err);
        }
    });

    $('a.comments').click(function () {
        try {
            hdrtitle = $(this).attr('hdrtitle');

        }
        catch (err) {
            alert(err);
        }

    });

    $('a.file_attach').click(function () {
        try {
            var DocumnetNo = $(this).attr('docno')
            if (DocumnetNo != "") {
                docno = $(this).attr('docno');
                doctype = $(this).attr('doctype');
                scrnid = $(this).attr('scrnid');
                hdrtitle = $(this).attr('hdrtitle');
            }
            else {

                kendoAlert("Task Id  cannot be blank", "Warning");
                return false;
            }
        }
        catch (err) {
            alert(err);
        }
    });

    $('a.report_modal').click(function () {
        hdrtitle = $(this).attr('hdrtitle');
        scrnid = $(this).attr('scrnid');
        //  alert(hdrtitle);


    });
    //Help

    $('a.HelpWindow').click(function () {
        try {
            hdrtitle = $(this).attr('hdrtitle');
            searchid = $(this).attr('searchid');
        } catch (err) {
            alert(err);
        }
    });

    $('a.Export').click(function () {
        try {
            hdrtitle = $(this).attr('hdrtitle');
            TreeId = $(this).attr('TreeId');
            SubTreeId = $(this).attr('SubTreeId');
        }
        catch (err) {
            alert(err);
        }
    });

    //$('#btnGridSave').on('click', function (evt) {
    //    var add_grid_id = evt.currentTarget.parentNode.parentNode.id;
    //    if (add_grid_id == "") {
    //        for (k = 0; k < $("#" + Form_validate_Name).find('.k-grid').length; k++) {
    //            add_grid_id = $("#" + Form_validate_Name).find('.k-grid')[k].id;
    //            grid_validation(add_grid_id);

    //        }
    //    }
    //    else {
    //        grid_validation(add_grid_id);
    //    }


    //})
    //setprofilePicture();
});

function masterNewView() {
    try {
        hdrtitle = $(this).attr('');
        cmbMasterId = $(this).attr('cmbMasterId');
    }
    catch (err) {
        alert(err);
    }
};

function load_combo_transaction(mstcode) {
    try {
        var data = {};
        var list = [];
        data.mstcode = mstcode;
        var parent_code = ajaxcall('/Home/mastercodelist');
        var mst_Dt = ajaxcall_param('/Home/fetchcombo', JSON.stringify(data));
        if (mst_Dt.toString() == "null" || mst_Dt == "[]") {
            var parent_code = ajaxcall('/Home/mastercodelist');
            mst_Dt = ajaxcall_param('/Home/fetchcombo', JSON.stringify(data));
        }
        if (mst_Dt.toString() != "null" && mst_Dt != "[]")
            list = getGridComboList(JSON.parse(mst_Dt));
        return list;
    }
    catch (err) {
        javascript_log4j_root(arguments.callee.name, err);
    }
}

function load_combo_mastercodes(mstcode) {
    var data = {};
    var list = [];
    data.mstcode = mstcode;
    var parent_code = ajaxcall('/Home/mastercodelist');
    var mst_Dt = ajaxcall_param('/Home/getcode', JSON.stringify(data));
    if (mst_Dt.toString() == "null" || mst_Dt == "[]") {
        var parent_code = ajaxcall('/Home/mastercodelist');
        mst_Dt = ajaxcall_param('/Home/getcode', JSON.stringify(data));
    }
    if (mst_Dt.toString() != "null" && mst_Dt != "[]")
        list = getGridComboList(JSON.parse(mst_Dt));//changed as getComboList
    return list;
}

function load_combo_master(mstcode) {
    var data = {};
    data.mstcode = mstcode;
    var parent_code = ajaxcall('/Home/mastercodelist');
    var mst_Dt = ajaxcall_param('/Home/fetchcombo', JSON.stringify(data));
    if (mst_Dt.toString() == "null" || mst_Dt == "[]") {
        var parent_code = ajaxcall('/Home/mastercodelist');
        mst_Dt = ajaxcall_param('/Home/fetchcombo', JSON.stringify(data));
    }
    if (mst_Dt.toString() != "null" && mst_Dt != "[]")
        list = getGridComboList(JSON.parse(mst_Dt));//changed as getComboList
    return list;
}


function grid_combo_mastercodes(mstcode) {
    var data = {};
    var list = [];
    data.mstcode = mstcode;
    var parent_code = ajaxcall('/Home/mastercodelist');
    var mst_Dt = ajaxcall_param('/Home/getcode', JSON.stringify(data));
    if (mst_Dt.toString() == "null" || mst_Dt == "[]") {
        var parent_code = ajaxcall('/Home/mastercodelist');
        mst_Dt = ajaxcall_param('/Home/getcode', JSON.stringify(data));
    }
    if (mst_Dt.toString() != "null" && mst_Dt != "[]")
        list = getGridComboList(JSON.parse(mst_Dt));//changed as getComboList
    return list;
}


function grid_comboScreen_mastercodes(mstcode) {
    var data = {};
    var list = [];
    data.mstcode = mstcode;
    var parent_code = ajaxcall('/Home/screenId_mastercodelist');
    var mst_Dt = ajaxcall_param('/Home/getScreenIDcode', JSON.stringify(data));
    if (mst_Dt.toString() == "null" || mst_Dt == "[]") {
        var parent_code = ajaxcall('/Home/screenId_mastercodelist');
        mst_Dt = ajaxcall_param('/Home/getScreenIDcode', JSON.stringify(data));
    }
    if (mst_Dt.toString() != "null" && mst_Dt != "[]")
        list = getGridComboList(JSON.parse(mst_Dt));//changed as getComboList
    return list;
}

// For Masster Code 
function grid_comboScreen_deptcodes(deptcode) {

    var data = {};
    var list = [];
    data.deptcode = deptcode;
    var parent_code = ajaxcall('/Home/screenId_mastercodelist');
    var mst_Dt = ajaxcall_param('/Home/getScreencode', JSON.stringify(data));
    if (mst_Dt.toString() == "null" || mst_Dt == "[]") {
        var parent_code = ajaxcall('/Home/screenId_mastercodelist');
        mst_Dt = ajaxcall_param('/Home/getScreencode', JSON.stringify(data));
    }
    if (mst_Dt.toString() != "null" && mst_Dt != "[]")
        list = getGridComboList(JSON.parse(mst_Dt));//changed as getComboList
    return list;
}

//grid level combo
function getGridComboList(mst_Dt) {
    var arr = [];
    $.each(mst_Dt, function (key, value) {
        var list = {};
        if (value != null) {
            list.code = value.code;            
            list.desc = (getlocalStorage("localeId") == 'en_US') ? value.description : value.lldescription;
            list.status = value.code_status;
            list.dependent = value.dependent_code;
        }
        arr.push(list);
    });
    return arr;
}

function grid_comboScreen_tamil(mstcode) {
    var data = {};
    var list = [];
    data.mstcode = mstcode;
    var parent_code = ajaxcall('/Home/screenId_mastercodelist');
    var mst_Dt = ajaxcall_param('/Home/getScreenIDcode', JSON.stringify(data));
    if (mst_Dt.toString() == "null" || mst_Dt == "[]") {
        var parent_code = ajaxcall('/Home/screenId_mastercodelist');
        mst_Dt = ajaxcall_param('/Home/getScreenIDcode', JSON.stringify(data));
    }
    if (mst_Dt.toString() != "null" && mst_Dt != "[]")
        list = getGridComboListtamil(JSON.parse(mst_Dt));//changed as getComboList
    return list;
}


//grid level combo
function getGridComboListtamil(mst_Dt) {
    var arr = [];
    $.each(mst_Dt, function (key, value) {
        var list = {};
        if (value != null) {
            list.code = value.code;
            list.desc = value.lldescription;
            list.status = value.code_status;
            list.dependent = value.dependent_code;
        }
        arr.push(list);
    });
    return arr;
}

function master_saveasdraft() {
    var exists = (typeof saveAsDraft === 'function');
    if (exists == true) {
        //$('.k-invalid-msg').remove();
        $('.k-widget.k-tooltip.k-tooltip-validation.k-invalid-msg').css('display', 'none');
        saveAsDraft();
        //savedata();
        if ($('#DocStat').val() == "Draft") {
            $("#bottom_menus").show();
        }
    }
    else {
        kendoAlert("Missing save As Draft function");
    }
}

function master_deletedraft() {
    var exists = (typeof deletedraft === 'function');
    if (exists == true) {
        $('.k-widget.k-tooltip.k-tooltip-validation.k-invalid-msg').css('display', 'none');
        deletedraft();

    }
    else {
        kendoAlert("Missing Delete draft function");
    }
}


function master_run() {
    var exists = (typeof run === 'function');
    if (exists == true) {
       
        run();

    }
    else {
        kendoAlert("Missing Run function");
    }
}



function master_save() {
    var exists = (typeof save === 'function');
    if (exists == true) {
        //$(':input').removeAttr('readonly');
        //grid_blank();
        for (var i = 0; i < $('label.required').length; i++) {
            var label_name = $('label.required:eq(' + i + ')');
            var label_for = label_name.attr('for');
            var input_tag = $("#" + label_for);
            var addclass = input_tag.addClass('required')
            var set_prop = $("#" + label_for + '.required').prop('required', 'required');
            var label_name = label_name.text();
            var str = label_name;
            var res = str.split(":");
            res = res[0];
            var data_role = $("#" + label_for + '.required').data("role");
            var set_custom_msg = "";
            if (data_role == "radio") {
                //set_custom_msg = $("#" + label_for + '.required').attr('data-radio-msg', 'Select any one  of the options');
                set_custom_msg = $("div input[data-role = 'radio']").attr('data-radio-msg', 'Select any one  of the options')
            }
            else if (data_role == "datepicker") {
                set_custom_msg = $("#" + label_for + '.required').attr('data-required-msg', '' + res + ' cannot be blank');
                set_custom_msg = $("#" + label_for + '.required').attr('data-checkdate-msg', 'Enter valid date');
            }
            else if (data_role == "checkbox") {
                set_custom_msg = $("#" + label_for + '.required').attr('data-required-msg', '' + res + ' cannot be uncheck');
            }
            else if (data_role == "filtercombo") {
                //$("#" + label_for + '.required').data("kendoComboBox").input.focus();
                $('#' + label_for).siblings().eq(0).children().eq(0).attr("tabindex", i + 1)
                set_custom_msg = $("#" + label_for + '.required').attr('data-required-msg', '' + res + ' cannot be blank');
            }
            else {
                var set_tabindex = $("#" + label_for + '.required').prop('tabindex', '' + (i + 1) + '');
                set_custom_msg = $("#" + label_for + '.required').attr('data-required-msg', '' + res + ' cannot be blank');
            }
        }
        var validator = $("#" + Form_validate_Name).data("kendoValidator");
           form_validate(validator);        
    }
    else {
        kendoAlert("Missing Save function");
    }
}

function form_validate(validator) {
    var valid_form = validator.validate();
    if (valid_form == false) {
        var txt_id = $('.k-invalid:eq(0)').attr('id');
        var data_role = $("#" + txt_id + '.required').data("role");
        var parent_tab = $('.k-invalid:eq(0)').parent().parent().parent().parent().parent().parent();
        var tab_id = parent_tab.attr('id');
        var parent_tab_id = parent_tab.parent().parent().parent().parent().parent().parent().attr('id');

        var p_t_id_li = $(".nav.nav-tabs a." + parent_tab_id).parent().parent().find("li.active").removeClass('active');
        var p_addclass_li = $(".nav.nav-tabs a." + parent_tab_id).parent().addClass('active');

        var p_removeclassbody = $(".tab-pane.fade #" + parent_tab_id).parent().parent().find(".active").removeClass('active in');
        var p_addclass_body = $(".tab-pane.fade #" + parent_tab_id).parent().addClass('active in');

        var t_id_li = $(".nav.nav-tabs a." + tab_id).parent().parent().find("li.active").removeClass('active');
        var addclass_li = $(".nav.nav-tabs a." + tab_id).parent().addClass('active');

        var removeclassbody = $(".tab-pane.fade #" + tab_id).parent().parent().find(".active").removeClass('active in');
        var addclass_body = $(".tab-pane.fade #" + tab_id).parent().addClass('active in');

        if (data_role == "filtercombo") {
            $("#" + txt_id).data("kendoComboBox").input.focus();
        }
        else {
            $("#" + txt_id).focus();
        }

        //var tabtext_id = $('.form-control.required.k-invalid:eq(0)').attr('id');
        //$("#" + tabtext_id).focus();

        grid_text_validation();
        return false;
        //if()
        //$(':input').attr('readonly', 'readonly');
        //$("#").removeAttr("readonly");
        //$("#").removeAttr("readonly");
        //$('.form-group .k-invalid-msg').remove()
    }
    else {
        save();
       
    }
}
function grid_text_validation() {
    var id_field = $('.k-invalid:eq(0)').parent().parent().parent().parent().parent().parent().parent().parent().parent().attr('id');

    var t_id_li = $(".nav.nav-tabs a." + id_field).parent().parent().find("li.active").removeClass('active');
    var addclass_li = $(".nav.nav-tabs a." + id_field).parent().addClass('active');

    var removeclassbody = $(".tab-pane.fade #" + id_field).parent().parent().find(".active").removeClass('active in');
    var addclass_body = $(".tab-pane.fade #" + id_field).parent().addClass('active in');

}


function grid_validation(add_grid_id) {
    var grid_data = $('#' + add_grid_id).data("kendoGrid");
    if (grid_data._data.length > 0) {
        var rows = grid_data.tbody.children();
        for (var i = 0; i < rows.length; i++) {
            var row = $(rows[i]);
            if (row.find('td:eq("1") input').val() == "") {
                grid_data.editCell(row.find('td:eq("1")'))
                return false;
            }

        }
    }
}

function master_new() {
    var exists = (typeof create_new === 'function');
    if (exists == true) {        
        $('.k-widget.k-tooltip.k-tooltip-validation.k-invalid-msg').css('display', 'none');
        create_new();
        
    }
    else {
        kendoAlert("Missing CreateNew function");
    }
}


function master_review() {
    var exists = (typeof review === 'function');
    if (exists == true) {
        $('.k-widget.k-tooltip.k-tooltip-validation.k-invalid-msg').css('display', 'none');
        review();

    }
    else {
        kendoAlert("Missing Ready for review function");
    }
}

function master_equipmentstart() {
    var exists = (typeof review === 'function');
    if (exists == true) {
        $('.k-widget.k-tooltip.k-tooltip-validation.k-invalid-msg').css('display', 'none');
        equipmentstart();

    }
    else {
        kendoAlert("Missing Equipment start function");
    }
}

function master_servicecompletion() {
    var exists = (typeof review === 'function');
    if (exists == true) {
        $('.k-widget.k-tooltip.k-tooltip-validation.k-invalid-msg').css('display', 'none');
        servicecompletion();

    }
    else {
        kendoAlert("Missing Accept Service Completion function");
    }
}

function master_approve() {
    var exists = (typeof approve === 'function');
    if (exists == true) {
        $('.k-widget.k-tooltip.k-tooltip-validation.k-invalid-msg').css('display', 'none');
        approve();

    }
    else {
        kendoAlert("Missing approve function");
    }
}


function master_reject() {
    var exists = (typeof reject === 'function');
    if (exists == true) {
        $('.k-widget.k-tooltip.k-tooltip-validation.k-invalid-msg').css('display', 'none');
        reject();

    }
    else {
        kendoAlert("Missing approve function");
    }
}


function master_saveandMaildraft() {
    var exists = (typeof saveandMaildraft === 'function');
    if (exists == true) {
        saveandMaildraft();
    }
    else {
        kendoAlert("Missing Save & Mail function");
    }
}



function master_saveAndPrintdraft() {
    var exists = (typeof saveAndPrintdraft === 'function');
    if (exists == true) {
        saveAndPrintdraft();
    }
    else {
        kendoAlert("Missing Save & Print function");
    }
}

function master_Cleardraft() {
    if ($('#modevalue').text() != "View Mode") {
        FormName = $('form.form-horizontal').attr('id');
        var formval = form_Serialize(FormName);
        var currentform_value = {};
        currentform_value.form = JSON.parse(formval);
        var gridData = [];
        $("div[data-role='grid']").each(function () {
            var id = $(this).attr("id");
            gridData.push(JSON.parse(JSON.stringify($("#" + id).data("kendoGrid").dataSource.data())));
        });
        currentform_value.grid = gridData;
        var dirty_cond = check_dirty(oldform_value, currentform_value);

        if (dirty_cond == true) {
            $("#dirtymodal_proceed").css("display", "none");
            $(".custom_proceed_href").css("display", "none");
            if ($(".custom_proceed_clear").length > 0) {
                $(".custom_proceed_clear").remove();
            }
            $("#dirty_modal_btn").prepend("<button id ='dirty_clear' type='button' class='custom_proceed_clear btn btn-danger' style='float:left;color:white;background-color:#F58322;border-color:#F58322' onclick='page_clear()'>Leave This Page</button>");
            $('#custom_dirty').modal("show");
            return false;
        }
        else {
            jQuery("#" + Form_validate_Name)[0].reset();
            for (k = 0; k < $("#" + Form_validate_Name).find('.k-grid').length; k++) {
                $("#" + Form_validate_Name).find('.k-grid:eq(' + k + ')').data("kendoGrid").dataSource.data([]);
            }
            $("#txtID").val('');
            $("#txtDate").val('');
            $("#DocStat").val("New");
            $(".k-invalid-msg").remove();
            //if(menuId=="CTG"){
            //$('#cmb_catg_applic').data("kendoComboBox").value('ERP_CATG');
            //}
        }
    }
    else {
        jQuery("#" + Form_validate_Name)[0].reset();
        for (k = 0; k < $("#" + Form_validate_Name).find('.k-grid').length; k++) {
            $("#" + Form_validate_Name).find('.k-grid:eq(' + k + ')').data("kendoGrid").dataSource.data([]);
        }
        $("#txtID").val('');
        $("#txtDate").val('');
        $("#DocStat").val("New");
        $(".k-invalid-msg").remove();
        //if (menuId == "CTG") {
        //$('#cmb_catg_applic').data("kendoComboBox").value('ERP_CATG');
        //}
    }

    // noncleargrid();

}

function deletedraft_clear() {
    jQuery("#" + Form_validate_Name)[0].reset();
    for (k = 0; k < $("#" + Form_validate_Name).find('.k-grid').length; k++) {
        $("#" + Form_validate_Name).find('.k-grid:eq(' + k + ')').data("kendoGrid").dataSource.data([]);
    }
    $("#txtID").val('');
    $("#txtDate").val('');
    $("#DocStat").val('');
    $(".k-invalid-msg").remove();
}


function page_clear() {
    jQuery("#" + Form_validate_Name)[0].reset();
    for (k = 0; k < $("#" + Form_validate_Name).find('.k-grid').length; k++) {
        $("#" + Form_validate_Name).find('.k-grid:eq(' + k + ')').data("kendoGrid").dataSource.data([]);
    }
    $('#custom_dirty').modal("hide");
    $("#txtID").val('');
    $("#txtDate").val('');
    $("#DocStat").val("New");
    $(".k-invalid-msg").remove();
    //if (menuId == "CTG") {
    //    $('#cmb_catg_applic').data("kendoComboBox").value('ERP_CATG');
    //}
}

function master_InActdraft() {
    var exists = (typeof InActdraft === 'function');
    if (exists == true) {
        $('.k-widget.k-tooltip.k-tooltip-validation.k-invalid-msg').css('display', 'none');
        InActdraft();
    }
    else {
        kendoAlert("Missing InActivate function");
    }
}

function master_Copy() {
    var exists = (typeof Copydraft === 'function');
    if (exists == true) {
        Copydraft();
    }
    else {
        kendoAlert("Missing Copy function");
    }
}

function master_Print() {
    var exists = (typeof Printdraft === 'function');
    if (exists == true) {
        Printdraft();
    }
    else {
        kendoAlert("Missing Print function");
    }
}

function master_form_refresh() {
    var exists = (typeof FormRefresh === 'function');
    if (exists == true) {
        FormRefresh();
    }
    else {
        kendoAlert("Missing Form Refresh function");
    }
}


function master_first() {
    var exists = (typeof next === 'function');
    if (exists == true) {
        $('.k-widget.k-tooltip.k-tooltip-validation.k-invalid-msg').css('display', 'none');
        next('first');
    }
    else {
        kendoAlert("Missing Next function");
    }
}

function master_previous() {
    var exists = (typeof next === 'function');
    if (exists == true) {
        $('.k-widget.k-tooltip.k-tooltip-validation.k-invalid-msg').css('display', 'none');
        next('previous');
    }
    else {
        kendoAlert("Missing Next function");
    }
}

function master_next() {
    var exists = (typeof next === 'function');
    if (exists == true) {
        $('.k-widget.k-tooltip.k-tooltip-validation.k-invalid-msg').css('display', 'none');
        next('next');
    }
    else {
        kendoAlert("Missing Next function");
    }
}

function master_last() {
    var exists = (typeof next === 'function');
    if (exists == true) {
        $('.k-widget.k-tooltip.k-tooltip-validation.k-invalid-msg').css('display', 'none');
        next('last');
    }
    else {
        kendoAlert("Missing Next function");
    }
}


function convert(str) {
    var date = new Date(str),
        mnth = ("0" + (date.getMonth() + 1)).slice(-2),
        day = ("0" + date.getDate()).slice(-2);
    return [date.getFullYear(), mnth, day].join("-");
}

function getFormated_StringDate(date) {   //get DD/MM/YYYY or MM/DD/YYYYY
    if (date != "" || date != undefined) {
        var d = date.split("/");
        //return d[1] + "/" + d[0] + "/" + d[2];

        return d[2] + "/" + d[1] + "/" + d[0];
    }

}




function dateTo_DDMMYYYY(date) {
    if (date != "" || date != undefined) {
        date = new Date(date);
        var day = date.getDate();
        if (day < 10) {
            day = "0" + day;
        }
        var month = date.getMonth() + 1;
        if (month < 10) {
            month = "0" + month;
        }
        return day + "/" + month + "/" + date.getFullYear();
    }
}

//checkbox function
function getChkValue_toArray(cls) {
    var sel = $(cls + ':checked').map(function (_, el) {
        return $(el).val();
    }).get();
    return sel;
}


function master_list_refresh() {
    var exists = (typeof listRefresh === 'function');
    if (exists == true) {        
        listRefresh();
    }
    else {
        kendoAlert("Missing List Refresh function");
    }
}


function master_cmbMaster_refresh() {
    var exists = (typeof cmbMasterRefresh === 'function');
    if (exists == true) {
        cmbMasterRefresh();
    }
    else {
        kendoAlert("Missing Master List Refresh function");
    }
}



//get pagewise details

var current_pageIndex = 0;
function getPagination(status) {
    var all_data = getlocalStorage("ls_pageList");//JSON.parse(localStorage.getItem("ls_pageList"));
    var result = "";
    if (status == "next") {
        if (all_data.length - 1 == current_pageIndex) {
            result = all_data[current_pageIndex];
        } else {
            current_pageIndex++;
            result = all_data[current_pageIndex];
        }

    } else if (status == "last") {
        current_pageIndex = all_data.length - 1;
        result = all_data[current_pageIndex];
    }
    else if (status == "first") {
        current_pageIndex = 0;
        result = all_data[0];
    }
    else {
        if (current_pageIndex == 0) {
            result = all_data[current_pageIndex];
        } else {
            current_pageIndex--;
            result = all_data[current_pageIndex];
        }
    }
    return result;
}



function setDefaultValues(e) {

    if (e.type != "click" || e.type == undefined) {

        if (e.action == "" || e.action == undefined) {
            for (var i = 0; i < e.sender._data.length; i++) {
                if (e.sender._data[i].mode_flag == "") {
                    e.sender._data[i].mode_flag = "I";
                    if (e.sender._data[i].mode_flag == "I") {
                        var rows = e.sender.tbody.children();
                        var row = $(rows[i]);
                        row.css('backgroundColor', 'lightgreen')
                    }

                    var rows = e.sender.tbody.children();

                    for (var j = 1; j < rows.length; j++) {
                        var row = $(rows[j]);
                        var dataItem = e.sender.dataItem(row);

                        if (dataItem.mode_flag == "I") {
                            row.css('backgroundColor', 'lightgreen')
                        }
                        else if (dataItem.mode_flag == "D") {
                            row.css('backgroundColor', '#ff3333')
                        }
                    }

                }
                else if (e.sender._data[i].mode_flag != "") {
                    if (e.sender._data[i].mode_flag == "S")
                        if (e.sender._cellId == "comp_change_Grid_active_cell" || e.sender._cellId == "grid_detail_form_active_cell")

                            e.sender._data[i].mode_flag = "S";

                        else
                            e.sender._data[i].mode_flag = "U";

                }
                //else if (e.sender._data[i].mode_flag != "") {
                //    if (e.model.mode_flag != "I")
                //        e.model.mode_flag = "U";
                //}
            }
        }
    }
}


function setColor(rows, resultData) {


    for (var i = 0; i < rows.length; i++) {
        var row = $(rows[i]);

        if (resultData[i].mode_flag == "I" || resultData[i].mode_flag == "T") {
            row.css('backgroundColor', 'lightgreen');
        }
        else if (resultData[i].mode_flag == "D") {
            row.css('backgroundColor', '#ff3333');
        }
    }
    var child_del = $('.k-grid-Delete').children('span:first');
    $('.k-grid-Delete').html(child_del);

    var child_copy = $('.k-grid-Copy').children('span:first');
    $('.k-grid-Copy').html(child_copy);
}

function exportfunction(e, chkall, chkfilter, chktemp, twokendobox, gd) {
    
    e.preventDefault();
    var sdata = {};
    var detail = {};
    if (chkall == true) {
        if (sdata.griddata == "" || sdata.griddata == undefined) {
            sdata.griddata = JSON.stringify(e.sender.dataSource._data);

        }
    }
    else if (chkfilter == true) {
        if (e.sender.dataSource._view.length == 0) {
            //if (twokendobox == false) {
            //    kendoAlert('Sorry, no data found in the grid to Export');
            //}
            //return false;
        }
        else {
            sdata.griddata = JSON.stringify(e.sender.dataSource._view);
        }
    }
    else {
        var cmb_temp = $('#cmbtemp').data("kendoComboBox").text();
        if (cmb_temp == "") {
            kendoAlert("template cannot be blank");
            return false;
        }
        else {
            var combo_temp = $('#cmbtemp').data("kendoComboBox");
            cmb_temp_path = $('#cmbtemp').data("kendoComboBox").dataSource.data()[combo_temp.selectedIndex].code;
            var path = cmb_temp_path.split("//");
            path = path[2].split(".");
            if (cmb_temp_path != "" && cmb_temp != "") {
                sdata.combovalue = cmb_temp;
                sdata.filepath = path[1];
                sdata.SubTreeId = SubTreeId;
                sdata.TreeId = TreeId;
                sdata.griddata = JSON.stringify(e.sender.dataSource._data);
            }
            else {
                if (twokendobox == false) {
                    kendoAlert('Please select any one of the template excel');
                }
                return false;
            }
        }
    }
    sdata.chkall = chkall;
    sdata.chkfilter = chkfilter;
    sdata.chktemp = chktemp;
    sdata.SubTreeId = SubTreeId;
    sdata.TreeId = TreeId;

    if (gd._data.length == 0) {
        if (twokendobox == false) {
            kendoAlert('Sorry, no data found in the grid to Export');
        }
        return false;
    }
    else {
        var export_excel_data = ajaxcall_param("/Home/Export_Excel", JSON.stringify(sdata));
        if (export_excel_data != undefined) {
            var exp_excel = JSON.parse(export_excel_data);
            if (exp_excel.success == true) {
                var pathname = window.location.pathname;
                var url = window.location.href;
                url = url.split("#");
                url = url[0];
                url = url.replace(pathname, "");
                window.location.href = url + (exp_excel.path);
            }
            if (twokendobox == false) {
                kendoAlert(exp_excel.msg);
            }
            e.preventDefault();
            return false;
        }
    }
}

//--Regarding for set the Combobox value and description.----//

function getSelectedIndex(control, property, cmbvalue) {
    var selectedIndex = -1;
    for (var i = 0; i < control.options.length; i++) {
        var comboVal = "";
        if (property == "code") {
            comboVal = control.options[i].value.toLowerCase();
            if (comboVal != "") {
                if (cmbvalue.toString().toLowerCase() == comboVal) {
                    selectedIndex = i;
                    break;
                }
            }
        }
        else if (property == "description") {
            comboVal = control.options[i].text.toLowerCase();
            if (comboVal != "") {
                if (cmbvalue.toString().toLowerCase() == comboVal) {
                    selectedIndex = i;
                    break;
                }
            }
        }
    }
    return selectedIndex;
}


function filterhover() {
    $(".k-dropdown-operator").css('display', 'none');

    $("#" + list_grid_id + " .k-input").attr('disabled', 'false');
    $('input').on('mouseenter', function (e) {

        $("#" + list_grid_id + " .k-input").prop('readonly', true);
        $("#" + list_grid_id + " .k-input").prop('disabled', false);
        $(this).attr('title', $(this).val());
        var gd = $("#" + list_grid_id).data("kendoGrid");
        $(gd.thead.find('th')).each(function () {
            $(this).prop('title', $(this).data('title'));
            // could also use $(this).data('field')
        });
        e.preventDefault();

    }).on('mousedown', function () {
        $("#" + list_grid_id + " .k-input").prop('disabled', true);
    });
    $('.k-grid th').on('mouseenter', function (e) {
        $(this).attr('title', $(this).val());
        var gd = $("#" + list_grid_id).data("kendoGrid");
        $(gd.thead.find('th')).each(function () {
            $(this).prop('title', $(this).data('title'));
            // could also use $(this).data('field')
        });
        e.preventDefault();
    })
}

function filter_row(gd) {
    for (var i = 1; i < gd.columns.length; i++) {


        var filter = $(gd.thead.find("th:not(.k-hierarchy-cell,.k-group-cell)")[i]).data("kendoFilterMenu");

        if (filter != undefined) {
            var type = filter.type;
            var value1 = filter.filterModel.filters[0].value.toString();
            var value2 = filter.filterModel.filters[1].value.toString();
            var field = filter.filterModel.filters[0].field;
            if ((value1.toString()).endsWith("(India Standard Time)")) {
                var date1 = kendo.parseDate(value1);
                value1 = kendo.toString(date1, "dd/MM/yyyy");
            }
            if ((value2.toString()).endsWith("(India Standard Time)")) {
                var date2 = kendo.parseDate(value2);
                value2 = kendo.toString(date2, "dd/MM/yyyy");
            }
            var operator1 = filter.filterModel.filters[0].operator;
            if (operator1 == "eq") {
                operator1 = "Equal to" + ":";
            }
            else if (operator1 == "neq") {
                operator1 = "Not equal to" + ":";
            }
            else if (operator1 == "startswith") {
                operator1 = "Starts with" + ":";
            }
            else if (operator1 == "contains") {
                operator1 = "Contains" + ":";
            }
            else if (operator1 == "doesnotcontain") {
                operator1 = "Doesn't Contain" + ":";
            }
            else if (operator1 == "endswith") {
                operator1 = "Ends with" + ":";
            }
            else if (operator1 == "gte") {
                if (type == "date") {
                    operator1 = "After or equal to" + ":";
                }
                else {
                    operator1 = "Greater than or equal to" + ":";
                }
            }
            else if (operator1 == "gt") {
                if (type == "date") {
                    operator1 = "After" + ":";
                }
                else {
                    operator1 = "Greater than" + ":";
                }
            }
            else if (operator1 == "lte") {
                if (type == "date") {
                    operator1 = "Before or equal to" + ":";
                }
                else {
                    operator1 = "Less than or equal to" + ":";
                }
            }
            else if (operator1 == "lt") {
                if (type == "date") {
                    operator1 = "Before" + ":";
                }
                else {
                    operator1 = "Less than" + ":";
                }
            }
            var operator2 = filter.filterModel.filters[1].operator;
            if (operator2 == "eq") {
                operator2 = "Equal to" + ":";
            }
            else if (operator2 == "neq") {
                operator2 = "Not equal to" + ":";
            }
            else if (operator2 == "startswith") {
                operator2 = "Starts with" + ":";
            }
            else if (operator2 == "contains") {
                operator2 = "Contains" + ":";
            }
            else if (operator2 == "doesnotcontain") {
                operator2 = "Doesn't Contain" + ":";
            }
            else if (operator2 == "endswith") {
                operator2 = "Ends with" + ":";
            }
            else if (operator2 == "gte") {
                if (type == "date") {
                    operator2 = "After or equal to" + ":";
                }
                else {
                    operator2 = "Greater than or equal to" + ":";
                }
            }
            else if (operator2 == "gt") {
                if (type == "date") {
                    operator2 = "After" + ":";
                }
                else {
                    operator2 = "Greater than" + ":";
                }
            }
            else if (operator2 == "lte") {
                if (type == "date") {
                    operator2 = "Before or equal to" + ":";
                }
                else {
                    operator2 = "Less than or equal to" + ":";
                }
            }
            else if (operator2 == "lt") {
                if (type == "date") {
                    operator2 = "Before" + ":";
                }
                else {
                    operator2 = "Less than" + ":";
                }
            }
            if (value1 != "" && value2 != "") {
                $('span[data-field = ' + field + '] .k-input').val(operator1 + " " + value1 + " " + filter.filterModel.logic + " " + operator2 + " " + value2);

            }
            else if (value1 == "" && value2 != "") {
                //$('input[data-text-field = ' + filter.filterModel.filters[0].field + ']').val(value1 + " " + operator2 + " " + value2);
                $('span[data-field = ' + field + '] .k-input').val(value1 + " " + operator2 + " " + value2);
            }
            else if (value1 != "" && value2 == "") {
                //$('input[data-text-field = ' + filter.filterModel.filters[0].field + ']').val(operator1 + " " + value1 + " " + value2);
                $('span[data-field = ' + field + '] .k-input').val(operator1 + " " + value1 + " " + value2);
            }
            else {
                //$('input[data-text-field = ' + filter.filterModel.filters[0].field + ']').val(value1 + " " + value2);
                $('span[data-field = ' + field + '] .k-input').val(value1 + " " + value2);
            }
        }
    }

}


//*Auto Generate Functionality work *//



////function getComboList_master(mst_Dt) {
////    var list = "";
////    list += "<option></option>";

////    $.each(mst_Dt, function (key, value) {
////        if (value != null) {
////            var colorCode = getComboColorCode(value.status);
////            list += '<option value=' + value.code + ' class=' + colorCode + '>' + value.description + '</option>';
////        }
////    });
////    return list;
////}

////function getComboColorCodetran(status) {
////    if (status == "Inactive") {
////        return "color_Red";
////    }
////    else {
////        return "color_Default"
////    }
////}

////function getComboList_tran(mst_Dt) {
////    var list = "";
////    list += "<option></option>";

////    $.each(mst_Dt, function (key, value) {
////        if (value != null) {
////            var colorCode = getComboColorCodetran(value.status);
////            //list += '<option value=' + value.code + ' class=' + colorCode + '>' + value.code + '</option>';             
////            list += '<option desc = ' + value.description + ' value=' + value.code + ' class=' + colorCode + '>' + value.code + '</option>';
////        }
////    });
////    return list;
////}

function masterNewView() {
    try {
        hdrtitle = $(this).attr('');
        cmbMasterId = $(this).attr('cmbMasterId');
    }
    catch (err) {
        alert(err);
    }
};

///refresh
function master_cmbMaster_refresh1(cmbMasterId) {
    var refesh = "_Refresh";
    window[cmbMasterId + refesh]();

}

//filtercombo
function filter_combobox(id, datasource) {
    $("#" + id).kendoComboBox({
        dataTextField: "desc",
        dataValueField: "code",
        filter: "contains",
        autoBind: false,
        minLength: 1,
        dataSource: datasource,
        change: function () {
            //  setColorforcombo();
            var value = this.value();
            var text = this.text();
            if (value && this.selectedIndex == -1) {
                this.value("");
            }
            if (id == "cmb_user") {
                setroleforinternal(text);
            }
        },
        dataBound: setColorforcombo,
        // dataBinding: setColor,
    });

    // var comboBox = $("#" + id).data("kendoComboBox");

    //$("#" + id + " .k-item").css("display", "inline-block");
    //$("#" + id + " .k-item").css( "min-width", "100%");

    //comboBox.list.css("min-width","100%");

    list_in = "";
}


function mul_filter_combobox(id, datasource) {
    $("#" + id).kendoMultiSelect({
        dataTextField: "desc",
        dataValueField: "code",
        autoClose: false,
        dataSource: datasource
    });

    list_in = "";
}

function setColorforcombo() {

    for (var i = 0; i < this.dataSource._data.length; i++) {
        var val = this.dataSource._data[i].status;
        var desc = this.dataSource._data[i].desc;
        var row = $(this.list.find('li')[i]);
        if (val == "Active") {
            row.css('color', 'black')
        }
        else if (val == "Inactive") {
            row.css('color', 'red')
        }
        //if (desc == "Internal") {
        //    row.css('display', 'none');
        //    var len = $('#cmb_user').data('kendoComboBox').dataSource._data.length;
        //    $('#cmb_user').data('kendoComboBox').dataSource.add({
        //        code: len + 1,
        //        desc: "Internal"
        //    })
        //    break;
        //    return false;
        //}
    }
}//filtercombo
function filter_combobox_code(id, datasource) {
    $("#" + id).kendoComboBox({
        dataTextField: "code",
        dataValueField: "code",
        filter: "contains",
        autoBind: false,
        minLength: 1,
        dataSource: datasource
    });
    list_in = "";
}

//calculation
function compute(id, grid_frmla_res, form_frmla_res) {
    var loop_result = 0; var g_result = 0; var f_result = 0;
    try {
        var grid = $("#" + id).data("kendoGrid");
        var gridData = grid.dataSource.view();

        for (var i = 0; i < gridData.length; i++) {
            for (var j = 0; j < grid_frmla_res.length; j++) {
                loop_result = eval(grid_frmla_res[j].grd_formula);
                if (loop_result != undefined && loop_result.toString() != "NaN")
                    eval(grid_frmla_res[j].grd_result + "=" + parseFloat(loop_result));
            }
            for (var k = 0; k < form_frmla_res.length; k++) {
                if (k == 0) {
                    var temp_rec = eval(form_frmla_res[k].frm_formula);
                    if (temp_rec != undefined) {
                        g_result += parseFloat(temp_rec);
                        if (g_result.toString() != "NaN")
                            document.getElementById(form_frmla_res[k].frm_result).value = g_result;
                    }
                }
                else {
                    f_result = eval(form_frmla_res[k].frm_formula);
                    if (f_result != undefined)
                        document.getElementById(form_frmla_res[k].frm_result).value = parseFloat(f_result);
                }
            }
        }
    }
    catch (err) {
        var error = err.message;
    }
}

function grid_refresh(id) {
    $('#' + id).data('kendoGrid').refresh();
}


$(function () {
    ////$('button').on('mouseenter', function (e) {
    ////    $(this).attr('title', $(this).text());
    ////});
});

//form header title //
function header_title(title, MenuId,ModuleId) {
    setlocalStorage('header_title', title);
    setlocalStorage('MenuId', MenuId);
    setlocalStorage('ModuleId', ModuleId);
    setlocalStorage('flt_condition', "");
    setlocalStorage('localeId', "en_US");
    // setlocalStorage('btn_draft_click', " ");
}

//grid header select all checkbox
//$(document).on("click", ".k-grid thead input:checkbox[id]", function () {
//    var grid_id = $(this).parent().parent().parent().parent().parent().parent().parent().attr('id');
//    var grid_id = $(this).parent().parent().parent().parent().parent().parent().parent().attr('class');
//    var child_class = $(this).attr('class').split(" ")[0];
//    var grid_data = $('#' + grid_id).data("kendoGrid");
//    //var dataItem = grid_data.dataItem($(this).closest('tr'));
//    var tbody_chkbx = $("#" + grid_id + " tbody ." + child_class + "");
//    var checked = $(this).is(':checked');
//    if (checked == true) {
//        $("tbody ." + child_class + "").click();
//        tbody_chkbx.prop("checked", true);

//    }
//    else {
//        $("tbody ." + child_class + "").click();
//        tbody_chkbx.prop("checked", false);
//        //$('.child1').click();
//    }
//});


//$(document).on("click", ".k-grid tbody input:checkbox[class]", function () {
//    var gid = $(this).parent().parent().parent().parent().parent().parent().attr('id');
//    var grid_class = $(this).attr('class').split(" ")[0];
//    var data_field = $("#" + gid + " .k-grid-header-wrap").find("th input:checkbox." + grid_class).parent().data("field");
//    var gdata = $('#' + gid).data("kendoGrid");
//    var dataItem = gdata.dataItem($(this).closest('tr'));
//    var checked = $(this).is(':checked');
//    if (checked == true) {
//        dataItem._set(data_field, '1');
//        //$("#" + gid + " .k-grid-header-wrap").find("th input:checkbox." + grid_class).prop("checked", true);
//    }
//    else {
//        dataItem._set(data_field, '0');
//        // $("#" + gid + " .k-grid-header-wrap").find("th input:checkbox." + grid_class).prop("checked", false);
//    }
//});

function check_dirty(oldform_value, currentform_value) {
    var formdirty = false;
    $.each(oldform_value.form, function (key, mytask) {
        $.each(currentform_value.form, function (skey, smytask) {
            if (key.toLowerCase() == skey.toLowerCase()) {
                if (mytask != smytask) {
                    formdirty = true;
                    return;
                }
            }
        });
    });
    if (oldform_value.grid != undefined) {
        for (var i = 0; i < oldform_value.grid.length; i++) {
            if (oldform_value.grid[i].length != currentform_value.grid[i].length) {
                formdirty = true;
            } else {
                $.each(oldform_value.grid[i], function (gkey, gvalue) {
                    var current = currentform_value.grid[i][gkey];
                    if ((current.mode_flag != undefined) && (current.mode_flag != "D")) {
                        current = currentform_value.grid[i][gkey];
                        current.mode_flag = "";
                    }
                    current = JSON.stringify(currentform_value.grid[i][gkey]);
                    var old = oldform_value.grid[i][gkey];
                    if (old.mode_flag != undefined) {
                        old = oldform_value.grid[i][gkey];
                        old.mode_flag = "";
                    }
                    old = JSON.stringify(oldform_value.grid[i][gkey]);
                    if (current != old) {
                        formdirty = true;
                        return;
                    }
                });
            }
        }
    }
    return formdirty;
}


function dirty_modal() {
    $('<div id="custom_dirty" class="modal fade" tabindex="-1" role="dialog" aria-labelledby="custom-title" data-backdrop="static" data-keyboard="false">' +
   '<div class="modal-dialog" role="document">' +
       '<div class="modal-content panel-danger" style="background-color:#F58322">' +
           '<div class="modal-header panel-heading" style="color:white;background-color:#F58322;border-color:#F58322">' +
               '<button type="button" class="close" data-dismiss="modal" aria-label="Close">' +
               '<span aria-hidden="true">×</span>' +
               '</button>' +
               '<h3 class="modal-title" id="custom-title"><span class="glyphicon glyphicon-exclamation-sign" aria-hidden="true"></span>Are you sure you want to do that?</h3>' +
           '</div>' +
           '<div class="modal-body panel-body dirty-message" style="background-color:white">You&#39;ve made changes on this page which aren&#39;t saved. If you leave you will lose these changes.</div>' +
           '<div class="modal-footer panel-footer">' +
           '<div class=""  id="dirty_modal_btn" style="float:right">' +
               '<button id ="dirtymodal_proceed" type="button" class="custom-proceed btn btn-danger" style="float:left;color:white;background-color:#F58322;border-color:#F58322">Leave This Page</button>' +
               '<button type="button" class="custom-stay btn btn-default" data-dismiss="modal" style="float:left">Stay Here</button>' +
           '</div>' +
     '</div>' +
       '</div>' +
   '</div>' +
'</div>').appendTo('body');
}

function onAnchorClick(ev) {
    if (menuId == "PO_WO_MAP" || menuId == "GRN_WO_MAP" || menuId == "ISU_WO_MAP" || menuId == "ISU_ISU_MAP" || menuId == "PO_PO_MAP" || menuId == "GRN_GRN_MAP") {
        if ($("#DocStat").val() == "Draft") {
            $("#dirtymodal_proceed").css("display", "none");
            $("#dirty_clear").css("display", "none");
            if ($(".custom_proceed_href").length > 0) {
                $(".custom_proceed_href").remove();
            }
            // $("#dirty_modal_btn").prepend("<button id ='dirty_draft' type='button' class='custom_proceed_draft btn btn-danger' style='float:left;color:white;background-color:#F58322;border-color:#F58322' onclick='return deletedraft()' href='" + form_list_url + "'>Leave This Page</button>");
            $("#dirty_modal_btn").prepend("<a  class='custom_proceed_href btn btn-danger' style='float:left;color:white;background-color:#F58322;border-color:#F58322' onclick='return deletemap()' href='" + ev.currentTarget.pathname + "'>Leave This Page</a>");
            $('#custom_dirty').modal("show");
            return false;
        }
        else {
            anchor_menu(ev);
            return false;
        }
    }
    else {
        anchor_menu(ev);
        return false;
    }
}


function anchor_menu(ev) {
    if (ev.currentTarget.className.trim() == "custom_proceed_href btn btn-danger") {
        //  window.location.href = ev.toElement.href;

        window.location.href = ev.currentTarget.pathname;
        // setlocalStorage('btn_draft_click', " ");
    }
    else {
        var target = ev.target;
        FormName = $('form.form-horizontal').attr('id');
        var formval = form_Serialize(FormName);
        var currentform_value = {};
        currentform_value.form = JSON.parse(formval);
        var gridData = [];
        $("div[data-role='grid']").each(function () {
            var id = $(this).attr("id");
            gridData.push(JSON.parse(JSON.stringify($("#" + id).data("kendoGrid").dataSource.data())));
        });
        currentform_value.grid = gridData;
        var dirty_cond = check_dirty(oldform_value, currentform_value);
        if (dirty_cond == true) {
            $("#dirtymodal_proceed").css("display", "none");
            $("#dirty_clear").css("display", "none");
            if ($(".custom_proceed_href").length > 0) {
                $(".custom_proceed_href").remove();
            }
            $("#dirty_modal_btn").prepend("<a  class='custom_proceed_href btn btn-danger' style='float:left;color:white;background-color:#F58322;border-color:#F58322' href='" + ev.currentTarget.pathname + "'>Leave This Page</a>");
            $('#custom_dirty').modal("show");
            return false;
        }
        else {
            //  window.location.href = ev.toElement.href;
            //window.
            window.location.href = ev.currentTarget.pathname;
            return true;
        }

    }
}

function getGridDataSource(ids) {
    var arr = [];
    if (typeof ids == "object") {
        for (var i = 0; i < ids.length; i++) {
            var grid = $("#" + ids[i]).data("kendoGrid").dataSource.data();
            grid = JSON.parse(JSON.stringify(grid));
            arr.push(grid);
        }
    } else {
        alert("send only array format data to getGridDataSource function")
    }
    return arr;
}

function grid_combo_master(mstcode) {
    var data = {};
    data.mstcode = mstcode;
    var mst_Dt = ajaxcall_param('/Home/fetchcombo', JSON.stringify(data));
    if (mst_Dt != undefined) {
        var list = getGridComboList(JSON.parse(mst_Dt));
        return list;
    }
}
function grid_combo_master_tran(mstcode) {
    var data = {};
    data.mstcode = mstcode;
    var mst_Dt = ajaxcall_param('/Home/fetchcombo', JSON.stringify(data));
    if (mst_Dt != undefined) {
        var list = getGridComboList(JSON.parse(mst_Dt));
        return list;
    }
}

var cur_column = 0;
function getcurrent_cell_id(grid_id) {
    var grid = $("#" + grid_id).data("kendoGrid");
    $(grid.tbody).on("click", "td",
        function (e) {
            cur_column = 0;
            var grd = $("#" + grid_id).data("kendoGrid");
            var row = grid.tbody.find("tr");
            var colIdx = $("td", row).index(this);
            cur_column = colIdx;
        });
    return cur_column;
}

// dashboard chart
function addDummyRecord(info) {
    var dumArr = [];
    var retObj = {};
    if (info.Record.length == undefined) {
        var obj = {};
        $.each(info.Record, function (key, value) {
            obj[key] = value;
        });
        var dum_obj = {};
        dum_obj.empty = "";
        dumArr.push(dum_obj);
        dumArr.push(obj);
        retObj.Record = dumArr;
        info = retObj;
    }
    return info;
}

//numeric validation


function numeric_editor1(container, field, maxlen) {
    $('<input required  maxlength="' + maxlen + '"  name="' + field + '"/>')
     .appendTo(container)
     .kendoNumericTextBox({
         //format: "{0:n0}",
         // decimals:0,
         min: 0,
         max: 32767,
         format: "#",
         decimals: 0,
         spinners: false
     });
}


function numeric_editor(container, field, maxlen, no_decimals) {
    $('<input maxlength="' + maxlen + '"  name="' + field + '"/>')
     .appendTo(container)
     .kendoNumericTextBox({
         min: 0,
         format: "#",
         decimals: 0,
         spinners: false
     });
}


function item_rate(container, field, maxlen, no_decimals) {
    var formatStr = "#";
    var dec_str = "######";
    if (no_decimals > 0)
        formatStr = formatStr + "." + dec_str.substr(0, no_decimals);
    $('<input maxlength="' + maxlen + '"  name="' + field + '"/>')
     .appendTo(container)
     .kendoNumericTextBox({
         min: 0,
         format: "{:n2}",


         // format: formatStr,
         //decimals: no_decimals,
         spinners: false
     });
}


function numeric_editor_dot(container, field, maxlen, no_decimals) {
    var formatStr = "#";
    var dec_str = "######";
    if (no_decimals > 0)
        formatStr = formatStr + "." + dec_str.substr(0, no_decimals);
    $('<input maxlength="' + maxlen + '"  name="' + field + '"/>')
     .appendTo(container)
     .kendoNumericTextBox({
         min: 0,

         format: formatStr,
         decimals: no_decimals,
         spinners: false
     });
}

function combo_editor(container, cmbid, datasource, datafield, code_datafield, grid_id) {
    
    $('<input  id="' + cmbid + '" data-text-field="desc" data-value-field="desc" data-bind="value:' + datafield + '"/>').appendTo(container).kendoComboBox({
        autoBind: false,
        filter: "contains",
        dataSource: datasource,
        change: function (e) {
            var cmb_value = this.value();
            if (cmb_value && this.selectedIndex == -1) {
                this.value("");
            }
            //else{
            var cmb_var = $("#" + grid_id).data("kendoGrid");
            var selectedItem = cmb_var.dataItem(cmb_var.select());
            var combobox_data = $("#" + cmbid).data("kendoComboBox");
            if (combobox_data != undefined && combobox_data.selectedIndex == -1) {
                selectedItem[datafield] = "";
                selectedItem[code_datafield] = "";
            }
            else {
                selectedItem[datafield] = combobox_data.dataItem(combobox_data.selectedIndex).desc;
                selectedItem[code_datafield] = combobox_data.dataItem(combobox_data.selectedIndex).code;
            }


            //}
        }
    });

    $("#" + grid_id).parent().children().find(".k-input").attr("readonly", "readonly");
}

function textarea_editor(container, field, maxlen) {
    $('<textarea data-bind="value:' + field + '" maxlength="' + maxlen + '" style="color:black" />').appendTo(container);
}
var cur_column = 0;
function getcurrent_cell_id(grid_id) {
    var grid = $("#" + grid_id).data("kendoGrid");
    $(grid.tbody).on("click", "td",
        function (e) {
            cur_column = 0;
            var grd = $("#" + grid_id).data("kendoGrid");
            var row = grid.tbody.find("tr");
            var colIdx = $("td", row).index(this);
            cur_column = colIdx;
        });
    return cur_column;
}
function display_cmbeditor(container, cmbid, datasource, datafield, grid_id, display_fldid) {

    getcurrent_cell_id(grid_id);
    $('<input  id="' + cmbid + '" data-text-field="code" data-value-field="code" data-bind="value:' + datafield + '"/>').appendTo(container)
    .kendoComboBox({
        autoBind: false,
        dataSource: datasource,
        change: function (e) {
            var cmb_value = this.value();
            var cmb_var = $("#" + grid_id).data("kendoGrid");
            var selectedItem = cmb_var.dataItem(cmb_var.select());
            var combobox_data = $("#" + cmbid).data("kendoComboBox");

            var cmb_desc = "";
            if (combobox_data != undefined) {
                if (combobox_data.selectedIndex != -1) {

                    selectedItem[datafield] = combobox_data.dataItem(combobox_data.selectedIndex).code;

                    if (display_fldid != "_DISPFLD_") {
                        var cmb_val = combobox_data.selectedIndex;
                        cmb_desc = combobox_data.dataSource._data[cmb_val].desc;
                        selectedItem[display_fldid] = cmb_desc;
                    }
                }
                else {
                    selectedItem[datafield] = "";

                    if (display_fldid != "_DISPFLD_") {
                        selectedItem[display_fldid] = "";
                        cmb_desc = "";
                    }
                }

                if (display_fldid != "_DISPFLD_") {
                    var colCountval = getcurrent_cell_id(grid_id) + 1;
                    if (colCountval != 0)
                        $('#' + grid_id).find('td').eq(colCountval).text(cmb_desc);
                }
            }
        }
    });

}

function date_editor(container, options) {
    $('<input  data-text-field="' + options.field + '" data-value-field="' + options.field + '" data-bind="value:' + options.field + '" data-format="' + options.format + '"/>')
        .appendTo(container)
        .kendoDatePicker({});
}


function listgrid_colgroup() {
    $("#" + list_grid_id + " .k-grid-header-wrap colgroup").remove();
    $("#" + list_grid_id + " .k-grid-header-wrap table").append('<colgroup></colgroup>');
    $("#" + list_grid_id + " .k-grid-content colgroup").remove();
    $("#" + list_grid_id + " .k-grid-content table").append('<colgroup></colgroup>');
    for (var i = 0; i < ($("#" + list_grid_id).data("kendoGrid").columns.length) ; i++) {
        if ($("#" + list_grid_id).data("kendoGrid").columns[i].hidden == false) {
            var width = $("#" + list_grid_id).data("kendoGrid").columns[i].width;
            $("#" + list_grid_id + " .k-grid-header-wrap colgroup").append('<col style = "width: ' + width + 'px">');
            $("#" + list_grid_id + " .k-grid-content colgroup").append('<col style = "width: ' + width + 'px">');
        }
    }
    $('#list_refresh').on('mouseenter', function (e) {
        var list_tooltip = $('#textserarch').val();
        $('#list_refresh').prop('title', list_tooltip);
    });
}

function combo_gen_editor(container, cmbid, datasource, datafield, grid_id) {

    $('<input  id="' + cmbid + '" data-text-field="desc" data-value-field="desc" data-bind="value:' + datafield + '"/>').appendTo(container).kendoComboBox({
        autoBind: false,
        dataSource: datasource,
        change: function (e) {
            var cmb_value = this.value();
            var cmb_var = $("#" + grid_id).data("kendoGrid");
            var selectedItem = cmb_var.dataItem(cmb_var.select());
            var combobox_data = $("#" + cmbid).data("kendoComboBox");
            if (combobox_data != undefined)
                selectedItem[datafield] = combobox_data.dataItem(combobox_data.selectedIndex).desc;
            // selectedItem[code_datafield] = combobox_data.dataItem(combobox_data.selectedIndex).code;
        }
    });
}

//security permission for list and grid

function page_load_list_permission(permission) {
    if (permission.deny == true) {
        $('#btn_new').attr("disabled", true);
        $('#btn_Edit').attr("disabled", true);
        $('#btn_View').attr("disabled", true);
        var data = [];
        listgrid(data);
    }
    else {
        if (permission.new == true) {
            $('#btn_new').attr("disabled", false);
            $('#btn_Edit').attr("disabled", true);
            $('#btn_View').attr("disabled", false);
        }
        if (permission.modify == true) {
            $('#btn_new').attr("disabled", true);
            $('#btn_Edit').attr("disabled", false);
            $('#btn_View').attr("disabled", false);
        }
        if (permission.delete == true) {
            $('#btn_new').attr("disabled", true);
            $('#btn_Edit').attr("disabled", false);
            $('#btn_View').attr("disabled", false);
        }

        if (permission.modify == true && permission.new == true && permission.delete == false) {
            $('#btn_new').attr("disabled", false);
            $('#divEdit #btnSaveEd').attr("disabled", false);
            $('#divEdit #deactivate').attr("disabled", true);
        }
        if (permission.new == true && permission.delete == true && permission.modify == false) {
            $('#btn_new').attr("disabled", false);
            $('#divEdit #btnSaveEd').attr("disabled", true);
            $('#divEdit #deactivate').attr("disabled", false);
        }
        if (permission.new == false && permission.delete == true && permission.modify == true) {
            $('#btn_new').attr("disabled", true);
            $('#divEdit #btnSaveEd').attr("disabled", false);
            $('#divEdit #deactivate').attr("disabled", false);
        }
        if (permission.new == true && permission.delete == true && permission.modify == true) {
            $('#btn_new').attr("disabled", false);
            $('#divEdit #btnSaveEd').attr("disabled", false);
            $('#divEdit #deactivate').attr("disabled", false);
        }
        if (permission.new == false && permission.delete == false && permission.modify == false && permission.auth == false && permission.view == true) {
            $('#btn_new').attr("disabled", true);
            $('#btn_Edit').attr("disabled", true);
            $('#btn_View').attr("disabled", false);
        }

        if (permission.new == false && permission.delete == false && permission.modify == false && permission.auth == true && permission.view == true) {
            $('#btn_new').attr("disabled", true);
            $('#btn_Edit').attr("disabled", false);
            $('#btn_View').attr("disabled", false);
        }

       

        if (Screen_Id == "FARMREG" || Screen_Id == "FARMPREF") {
            listpageloadfetch(0, 20);
        }
        else {
            listpageloadfetch();
        }

    }
    return false;
}




function form_user_permission(permission) {

    if (permission.modify == true) {
        $('#divEdit #btnSaveDraft').attr("disabled", false);
        $('#divEdit #btnDeleteDraft').attr("disabled", true);
        $('#divEdit #btnSave').attr("disabled", false);
        $('#divEdit #inactivate').attr("disabled", true);
    }
    if (permission.delete == true) {
        $('#divEdit #btnSaveDraft').attr("disabled", true);
        $('#divEdit #btnDeleteDraft').attr("disabled", false);
        $('#divEdit #btnSave').attr("disabled", true);
        $('#divEdit #inactivate').attr("disabled", false);
    }
    if (permission.modify == true && permission.new == true && permission.delete == false) {
        $('#divEdit #btnSaveDraft').attr("disabled", false);
        $('#divEdit #btnDeleteDraft').attr("disabled", true);
        $('#divEdit #btnSave').attr("disabled", false);
        $('#divEdit #inactivate').attr("disabled", true);
    }
    if (permission.new == true && permission.delete == true && permission.modify == false) {
        $('#divEdit #btnSaveDraft').attr("disabled", true);
        $('#divEdit #btnDeleteDraft').attr("disabled", false);
        $('#divEdit #btnSave').attr("disabled", true);
        $('#divEdit #inactivate').attr("disabled", false);
    }
    if (permission.new == false && permission.delete == true && permission.modify == true) {
        $('#divEdit #btnSaveDraft').attr("disabled", false);
        $('#divEdit #btnDeleteDraft').attr("disabled", false);
        $('#divEdit #btnSave').attr("disabled", false);
        $('#divEdit #inactivate').attr("disabled", false);

    }
    if (permission.new == true && permission.delete == true && permission.modify == true) {
        $('#divEdit #btnSaveDraft').attr("disabled", false);
        $('#divEdit #btnDeleteDraft').attr("disabled", false);
        $('#divEdit #btnSave').attr("disabled", false);
        $('#divEdit #inactivate').attr("disabled", false);
    }
}


//Grid: Pagination selected --start
var grid_selected_rows = {};
function reset_Selected_GridRows(id, o) {
    var grid = $("#" + id).data("kendoGrid");
    var newSelection = [];
    var pageData = o.sender.dataSource._data;
    if ($.isEmptyObject(grid_selected_rows) == false) {
        $.each(grid.dataSource._data, function (key, value) {
            $.each(grid_selected_rows, function (skey, svalue) {
                if ((grid_selected_rows[skey] == true) && (skey == value.randNum)) {
                    newSelection.push(pageData[key].uid);
                }
            });
        });
        $.each(newSelection, function (key, uid) {
            $("tr[data-uid='" + uid + "'] td:first").find("input[type='checkbox']").prop("checked", true);
            $("tr[data-uid='" + uid + "']").addClass("k-state-selected");
        });
    }
}

function selected_Grid_Row(id, e, rSelect, sel_Row) {
    var checked = e.checked;
    if (rSelect) {
        checked = true;
        row = sel_Row;
    } else {
        row = $(e).closest("tr");
    }
    grid = $("#" + id).data("kendoGrid");
    var selection = $(e).closest("tr");
    if (rSelect) {
        selection = sel_Row;
    }
    if (checked) {
        //-select the row
        row.addClass("k-state-selected");
        grid_selected_rows[grid.dataItem(selection).randNum] = true;
    } else {
        //-remove selection
        row.removeClass("k-state-selected");
        grid_selected_rows[grid.dataItem(selection).randNum] = false;
    }
}


function addRandomNum(data) {
    $.each(data, function (key, value) {
        var randNum = Math.floor((Math.random() * 1000000) + 100);
        data[key].randNum = randNum;
    })
    return data;
}
//Grid: Pagination selected--end


//grid mandatory set color
function grid_mandatory(grid_id, title) {

    $("#" + grid_id + " th[role='columnheader']").each(function () {
        for (var i = 0; i < title.length; i++) {
            if (title[i] == $(this).text()) {
                if ($(this).find("a").length > 0) {
                    $(this).children().append("<span class='cr_required'></span>");
                } else {
                    $(this).append("<span class='cr_required'></span>");
                }
            }
        }
    });
}


//mandatory for grid combo
function combo_editor_man(container, cmbid, datasource, datafield, code_datafield, grid_id) {

    $('<input  id="' + cmbid + '" data-text-field="desc" data-value-field="desc" data-bind="value:' + datafield + '" name ="' + datafield + '"/>').appendTo(container).kendoComboBox({
        autoBind: false,
        filter: "contains",
        dataSource: datasource,
        change: function (e) {
            var cmb_value = this.value();
            if (cmb_value && this.selectedIndex == -1) {
                this.value("");
            }
            //else{FF
            var cmb_var = $("#" + grid_id).data("kendoGrid");
            var selectedItem = cmb_var.dataItem(cmb_var.select());
            var combobox_data = $("#" + cmbid).data("kendoComboBox");
            if (combobox_data != undefined && combobox_data.selectedIndex == -1) {
                selectedItem[datafield] = "";
                selectedItem[code_datafield] = "";
            }
            else {
                selectedItem[datafield] = combobox_data.dataItem(combobox_data.selectedIndex).desc;
                selectedItem[code_datafield] = combobox_data.dataItem(combobox_data.selectedIndex).code;
            }


            //}
        }
    });

    $("<span class='k-invalid-msg' data-for='" + datafield + "'></span>").appendTo(container);
}

function grid_listcolumn(gd) {
    filter_row(gd);
    $('.k-i-close').css('display', 'none');
    $('button.k-button-icon').css('display', 'none');
    $('#grid_mst_category_list .k-input').prop('disabled', false);
    $('#grid_mst_category_list .k-input').prop('readonly', true);

    var fldval = $('#cmb_view_master').children();
    if (fldval != null && fldval.length == 0) {
        LoadView();
    }
    $('#cmb_view_master').attr('disabled', false);
    $(".Export").css("pointer-events", "");
    $(".Export").css("opacity", "");

    $("div.k-group-indicator").each(function (i, v) {
        var options = $('#cmb_view_master').children();
        for (var i = 0; i < options.length; i++) {
            if (options[i].innerHTML === "Groupview") {
                options[i].selected = true;
                View_master_selectChange("Groupview");
                break;
            }

        }
        $('#cmb_view_master').attr('disabled', true);
        $(".Export").css("pointer-events", "none");
        $(".Export").css("opacity", "0.3");

    })
}



function master_get_tamil() {
    var exists = (typeof get_tamil_lang === 'function');
    if (exists == true) {
        get_tamil_lang();
    }
    else {
        //kendoAlert("Missing Save & Print function");
        $("#MEnglish").css("display", "none");
        $("#MTamil").css("display", "block");
    }
}

function master_get_english() {
    var exists = (typeof get_english_lang === 'function');
    if (exists == true) {
        get_english_lang();
    }
    else {
        $("#MEnglish").css("display", "block");
        $("#MTamil").css("display", "none");
        //   kendoAlert("Missing Save & Print function");
    }
}



var arr = [];
var grid = "";
var ret_val = "";

function getLanguageListMode(screen_id, grid_id, def, loc) {

    $('[data-toggle="tooltip"]').tooltip();
    try {
        ret_val = "";
        var data = {};
        data.screen_id = screen_id;
        data.grid_id = grid_id;
        var context = WebAPIProxy.getContext();
        data.activity_code = 'FARMPREF';
        data.orgnid = context.orgnId;
        data.locnid = context.locnId;
        data.userid = context.userId;
        data.localeid = context.localeId;
        data.xml_name = "FieldGenerator.xml";
        var values = ajaxcall_param_new("/DynamicGrid/gridListCreation", data);
        var result = JSON.parse(values);
        // arr = [];
        for (var i = 0 ; i < result.length ; i++) {
            if (result[i].control_type == 'CTRL_TAB') {
                var a = $("#tab_list").find('li');
                for (var l = 0 ; l < a.length ; l++) {
                    if (a[l].innerHTML.search(grid_id) >= 0) {
                        if (result[i].language == def)
                            a[l].innerHTML = '<a href="#' + result[i].control_code + '"' + 'role="tab" data-placement="top" data-toggle="tab" data-original-title="' + result[i].control_desc.toString() + '"> ' + result[i].control_defdesc.toString() + '</a>'
                        else if (result[i].language == loc)
                            a[l].innerHTML = '<a href="#' + result[i].control_code + '"' + 'role="tab" data-placement="top" data-toggle="tab" data-original-title="' + result[i].control_defdesc.toString() + '"> ' + result[i].control_desc.toString() + '</a>'
                    }
                }
                $('[data-toggle="tab"]').tooltip();
            }
            else if (result[i].control_type == 'CTRL_GRID') {
                var fld = $("#" + result[i].control_code).attr('id');
                grid = $("#" + grid_id).data("kendoGrid");
                if (fld != 'undefined') {
                    if (result[i].language == def) {

                        if (result[i].control_id.toString() == grid_id) {
                            for (var j = 0; j < grid.columns.length ; j++) {
                                if (grid.columns[j].field == result[i].control_code.toString())
                                    $("#" + grid_id + " thead [data-field= " + result[i].control_code.toString() + "] .k-link").html(result[i].control_desc.toString());
                            }
                        }
                    }
                    if (result[i].language == loc) {

                        if (result[i].control_id.toString() == grid_id) {
                            arr.push(result[i]);
                        }

                    }
                }
            }

            else {

                if (result[i].language == def) {
                    if (result[i].control_type.toString() == "CTRL_LABEL")
                        $("#" + result[i].control_code).text(result[i].control_desc);
                }
                if (result[i].language == loc) {
                    if (result[i].control_type.toString() == "CTRL_TEXT") {
                        $("#" + result[i].control_code).attr("data-original-title", result[i].control_desc);
                    }
                    else if (result[i].control_type.toString() == "CTRL_COMBO") {
                        $("#" + result[i].control_code).attr("data-original-title", result[i].control_desc);
                    }
                }
            }
        }
        if (arr.length > 0) {
            grid.thead.kendoTooltip({
                filter: "th",
                content: function (e) {
                    var target = e.target;
                    var fld_name = target.context.attributes[1].value;
                    var fld_desc_val = arr;
                    ret_val = "";
                    $.each(fld_desc_val, function (key, value) {
                        if (fld_desc_val[key].control_code == fld_name) {
                            ret_val = fld_desc_val[key].control_desc;
                        }
                    });
                    return ret_val;
                }
            });
        }
    }
    catch (err) {
        //javascript_log4j_root(arguments.callee.name, err);
    }
}

function SetMakerCheckerPerm() {
    var userid = getlocalStorage('User_Id_Value');
    if (userid.toUpperCase() == "FPOCHECKER") {
        $("#divCreate").hide();
        $("#divReView").show();
        $("#divEdit").hide();

    }
    else {
        $("#divCreate").show();
        $("#divReView").hide();

    }

}


function changedataType(res) {
    $.each(res, function (key, value) {
        $.each(value, function (skey, svalue) {
            if (svalue == null)
                svalue = "";
            value[skey] = svalue.toString();
        })
        res[key] = value;
    });
    return res;
}

function getStartIndex(pager_id) {
    var st_index;
    var pageno = $("#" + pager_id).data("kendoPager").dataSource.page();
    var pageSize = 20;//$("#" + pager_id).data("kendoPager").dataSource.pageSize();
    if (pageno > 0 && pageSize > 0)
        st_index = ((pageno - 1) * pageSize) + 1;
    else
        st_index = 1;
    //  alert(pageno + pageSize);  
    return st_index;
}

function getEndIndex(pager_id, st_index) {
    var end_index;
    if (st_index == undefined)
        st_index = 1;
    //var pageno = $("#" + pager_id).data("kendoPager").dataSource.page();
    var pageSize = 20;//$("#" + pager_id).data("kendoPager").dataSource.pageSize();
    if (pageSize > 0)
        end_index = st_index + (pageSize - 1);
    else
        end_index = st_index;
    //  alert( pageSize);  
    return end_index;
}

