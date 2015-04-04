

var sCoreExi = {};

sCoreExi = function () {

    var _pub = {};
    var _actionRoot;
    var _canvasData = {};
    var _wizardData = {};
    var _canvasContainer = {};
    var _activeSectionIndex = 0;


    var _wizardSelectedWidgetGroupIndex = null;
    var _wizardSelectedWidgetIndex = null;
    var _wizardSelectedRendererIndex = null;

    var _onActiveSectionRendered = null;

    var _nonTileMode = false;

    var _failedRetryCount = 0;

    var _staticPositioning = false;

    var _pointerX;
    var _pointerY;

    var _zoomLevel = 1;

    /********************************************/
    /******* Canvas Functionality **************/
    /********************************************/

    _pub.initCanvas = function (container, options) {

        _actionRoot = options["ActionRoot"];
        _onActiveSectionRendered = options["onActiveSectionRendered"];

        if (options["nonTileMode"] != 'undefined') {
            _nonTileMode = options["nonTileMode"];
        }

        if (options["zoom"] != 'undefined') {
            _zoomLevel = options["zoom"];
        }

        $.ajaxSetup({
            cache: false
        });



        _canvasContainer = container;

        $.getJSON(_actionRoot + 'ExiGetCanvasJson', function (data) {
            _canvasData = data;

            for (i = 0; i < _canvasData["Sections"].length; i++) {
                if (_canvasData["Sections"][i]["Identifier"] == _canvasData["DefaultSection"]) {
                    _activeSectionIndex = i;
                }
            }

            sCoreExi.createCanvas();
            sCoreExi.renderActiveSection();
            sCoreExi.refreshSectionSelector();
        });

        $.getJSON(_actionRoot + 'ExiGetWizardDefinition', function (data) {
            _wizardData = data;
            sCoreExi.buildWizard();
        });

    }



    _pub.loadSlotContent = function (id, markTaskComplete, retry) {

        var domID = 'sCoreExiSlot_' + id;

        $.ajax(_actionRoot + 'ExiRenderSlot/?id=' + id, {
            timeout: 20000,
            context: $('#' + domID).find('.sCoreExiContent'),
            success: function (data) {

                var domId = $(this).closest('.sCoreExiPortlet').attr('id');
                var id = domId.replace('sCoreExiSlot_', '');
                $(this).html(data);

                if (markTaskComplete) {
                    sCoreExiTaskmgr.markTaskComplete(id);
                }

            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.log('sCore.Exi.Error.AjaxError=' + textStatus);
                var domId = $(this).closest('.sCoreExiPortlet').attr('id');
                var id = domId.replace('sCoreExiSlot_', '');

                if (markTaskComplete) {
                    sCoreExiTaskmgr.markTaskComplete(id);
                }

                if (retry) {

                    if (_failedRetryCount < 15) {
                        _failedRetryCount++;
                        console.log('sCore.Exi.Error.Misc=Initiating retry ' + _failedRetryCount + '....');
                        sCoreExi.loadSlotContent(id, false, true);
                    }
                    else {
                        console.log('sCore.Exi.Error.Misc=Max retry met');
                    }

                    
                }
            }
        });

    }


    _pub.setParentSlotForAutoRefresh = function (src, seconds) {
        var domId = $(src).closest('.sCoreExiPortlet').attr('id');
        var id = domId.replace('sCoreExiSlot_', '');
        var func = 'sCoreExi.loadSlotContent("' + id + '",true,false)';
        sCoreExiTaskmgr.scheduleTask(id, func, seconds);
    }


    _pub.refreshCanvasData = function () {
        $.getJSON(_actionRoot + 'ExiGetCanvasJson', function (data) {
            _canvasData = data;
            sCoreExi.refreshSectionSelector();
        });
    }

    _pub.refreshSectionSelector = function () {

        $('.sCoreExiSectionSelector').html('');

        for (i = 0; i < _canvasData["Sections"].length; i++) {

            var option = $('<option />').html(_canvasData["Sections"][i]["Name"])
            .attr("value", _canvasData["Sections"][i]["Identifier"]);

            if (_canvasData["Sections"][i]["IsShared"] == true) {
                $('<span/>')
                    .html('&nbsp;(Shared)')
                    .appendTo(option);
            }

            $(option).appendTo($('.sCoreExiSectionSelector'));

            var activeSection = _canvasData["Sections"][_activeSectionIndex];
            $('.sCoreExiSectionSelector').val(activeSection['Identifier']);
        }
    }

    _pub.changeSelectedSection = function () {
        var activeGuid = $('.sCoreExiSectionSelector').val();

        for (i = 0; i < _canvasData["Sections"].length; i++) {
            if (_canvasData["Sections"][i]["Identifier"] == activeGuid) {
                _activeSectionIndex = i;
                sCoreExi.renderActiveSection();
                return true;
            }
        }

        return true;
    }

    /* This is called internally when the canvas is initialized. It creates columns and DOM structured
        within the specified container */
    _pub.createCanvas = function () {


        var controlContainer = $('<div />');
        controlContainer.addClass('sCoreExiControlContainer');
        $(controlContainer).appendTo(_canvasContainer);

        var controlPanel = $('<div />');
        controlPanel.addClass('sCoreExiControlPanel');
        $(controlPanel).appendTo(controlContainer);

        $('<span>&nbsp;</span>').appendTo(controlPanel);

        /* Add Widget Button */

        var addWidgetButton = $('<button />');
        addWidgetButton.html('&nbsp;Add Item');
        addWidgetButton.css('font-size', '9px');
        addWidgetButton.css('z-index', '900');
        addWidgetButton.appendTo(controlPanel);
        addWidgetButton.addClass('sCoreExiControlAddWidget');
        addWidgetButton.click(function () { sCoreExi.showWizard() });

        $(addWidgetButton).button({
            icons: {  primary: "ui-icon-circle-plus" }
        })

        /* Print management panel */

        var printManagementPanel = $('<span />');
        printManagementPanel.css('float', 'right');
        printManagementPanel.css('display', 'none');
        printManagementPanel.addClass('sCoreExiSectionPrintManagement');
        $(printManagementPanel).appendTo(controlPanel);


        var printSelectedButton = $('<button />');
        printSelectedButton.html('&nbsp;Print Preview');
        printSelectedButton.css('font-size', '9px');
        printSelectedButton.appendTo(printManagementPanel);
        printSelectedButton.click(function () { sCoreExi.openPrintMode() });

        $(printSelectedButton).button({
            icons: { primary: "ui-icon-print" }
        })

        var clearSelectedButton = $('<button />');
        clearSelectedButton.html('&nbsp;Cancel');
        clearSelectedButton.css('font-size', '9px');
        clearSelectedButton.appendTo(printManagementPanel);
        clearSelectedButton.click(function () { sCoreExi.clearSelectedPrintSlots() });

        $(clearSelectedButton).button({
            icons: { primary: "ui-icon-close" }
        })

        /* Section management Panel */

        var sectionManagementPanel = $('<span />');
        sectionManagementPanel.css('float', 'right');
        sectionManagementPanel.addClass('sCoreExiSectionSectionManagement');
        $(sectionManagementPanel).appendTo(controlPanel);

        var sectionSelector = $('<select />');
        sectionSelector.addClass('sCoreExiSectionSelector');
        $(sectionSelector).appendTo(sectionManagementPanel);
        $(sectionSelector).change(function () { sCoreExi.changeSelectedSection() });


        var setDefaultButton = $('<button />');
        setDefaultButton.html('&nbsp;Default');
        setDefaultButton.css('font-size', '9px');
        setDefaultButton.appendTo(sectionManagementPanel);
        setDefaultButton.click(function () { sCoreExi.setDefaultSection() });

        $(setDefaultButton).button({
            icons: { primary: "ui-icon-pin-s" }
        })

        var addSharedSectionButton = $('<button />');
        addSharedSectionButton.html('&nbsp;Shared');
        addSharedSectionButton.css('font-size', '9px');
        addSharedSectionButton.addClass('sCoreExiControlAddSharedSection');
        addSharedSectionButton.appendTo(sectionManagementPanel);
        addSharedSectionButton.click(function () { sCoreExi.addSharedSection(); });

        $(addSharedSectionButton).button({
            icons: { primary: "ui-icon-circle-plus" }
        })


        var renameSharedSectionButton = $('<button />');
        renameSharedSectionButton.html('&nbsp;Rename');
        renameSharedSectionButton.css('font-size', '9px');
        renameSharedSectionButton.appendTo(sectionManagementPanel);
        renameSharedSectionButton.addClass('sCoreExiControlSectionRename');
        renameSharedSectionButton.click(function () { sCoreExi.renameSharedSection(); });

        $(renameSharedSectionButton).button({
            icons: { primary: "ui-icon-pencil" }
        })

    }

    _pub.showActiveSection = function () {
        $('.sCoreExiColumn').show();
    }

    _pub.hideActiveSection = function () {
        $('.sCoreExiColumn').hide();
    }


    _pub.refreshActiveSection = function () {

        $.getJSON(_actionRoot + 'ExiGetCanvasJson', function (data) {
            _canvasData = data;
            sCoreExi.renderActiveSection();
        });

    }

    /* This will identify the currently selected section and render the slots assigned to it */
    _pub.renderActiveSection = function () {

        var activeSection = _canvasData["Sections"][_activeSectionIndex];

        $('.sCoreExiColumn').remove();

        var columnContainer = $('<div />');

        var widthPerc = 100 / activeSection["Columns"].length;

        if (activeSection["Columns"].length < 2)
        {
            _staticPositioning = true;
        }
        else
        {
            _staticPositioning = false;
        }

        for (var i = 0; i < activeSection["Columns"].length ; i++) {
            $('<div />')
            .addClass('sCoreExiColumn')
            .css('width', widthPerc + '%')
            .appendTo(columnContainer);
        }

        $(columnContainer).appendTo(_canvasContainer);

        sCoreExi.enableEditingCanvas();

        for (var i = 0; i < activeSection["Columns"].length; i++) {

            var currentColumn = activeSection["Columns"][i];

            for (var ii = 0; ii < currentColumn["Slots"].length; ii++) {



                sCoreExi.createSlot(currentColumn["Slots"][ii], i);
                sCoreExi.loadSlotContent(currentColumn["Slots"][ii]['Identifier'], false, true);
            }
        }


        if (_onActiveSectionRendered != null) {
            _onActiveSectionRendered(activeSection);
        }

    }

    _pub.createSlot = function (slot,column) {
        
        var divId = 'sCoreExiSlot_' + slot['Identifier'];

        var slotElement = $('<span />')
        .addClass('sCoreExiPortlet')
        .attr('id', divId);

        if (_staticPositioning) {
            $(slotElement).css('position', 'absolute');
            $(slotElement).css('top', slot['Y'] + 'px');
            $(slotElement).css('left', slot['X'] + 'px');
        }

        if (!_nonTileMode) {

            var titleElement = $('<div />')
            .css('width', '80%')
            .html(slot['Title']);

            $('<div />')
            .addClass('sCoreExiHeader')
            .append(titleElement)
            .appendTo(slotElement);
        }

        $('<div />')
        .addClass('sCoreExiSlotControls')
        .appendTo(slotElement)
        .css('position', 'absolute')
        .css('z-index', '99999')
        .hide();

        $('<div />')
        .addClass('sCoreExiContent')
        .appendTo(slotElement);



        var columnContainer = $(_canvasContainer).find('.sCoreExiColumn')[column];   
        $(columnContainer).append(slotElement);

        $('#' + divId)
          .addClass("ui-widget ui-widget-content ui-helper-clearfix ui-corner-all")
          .find(".sCoreExiHeader")
            .addClass("ui-widget-header ui-corner-all");





        var removeButton = $('<button />');
        removeButton.html('&nbsp;Remove');
        removeButton.css('font-size', '9px');
        removeButton.appendTo($('#' + divId).find(".sCoreExiSlotControls"));
        removeButton.addClass('sCoreExiControlRemoveSlot');

        removeButton.click(function () {
            var result = confirm("Are you sure you want to remove this item?");
            if (result == true) {
                sCoreExi.removeSlot(this);
            }
        });

        var printButton = $('<button />');
        printButton.html('&nbsp;Print');
        printButton.css('font-size', '9px');
        printButton.css('display', 'none');
        printButton.appendTo($('#' + divId).find(".sCoreExiSlotControls"));
        printButton.addClass('sCoreExiControlPrintSlot');
        printButton.click(function () { sCoreExi.togglePrintSlot(this) });

        if (!_nonTileMode) {
            var editButton = $('<button />');
            editButton.html('&nbsp;Edit Title');
            editButton.css('font-size', '9px');
            editButton.appendTo($('#' + divId).find(".sCoreExiSlotControls"));
            editButton.addClass('sCoreExiControlEditSlotTitle');
            editButton.click(function () { sCoreExi.renameSlot(this) });
        }


        $(removeButton).button({
            icons: { primary: "ui-icon-close" }
        })

        $(printButton).button({
            icons: { primary: "ui-icon-print" }
        })

        $(editButton).button({
            icons: { primary: "ui-icon-pencil" }
        })

        $('#' + divId).find('.sCoreExiContent').html('Loading.....');

        $('#' + divId).mouseover(function() {
            $('.sCoreExiSlotControls').hide();
            $(this).find('.sCoreExiSlotControls').show();
        });

        $('#' + divId).mouseout(function () {
            $(this).find('.sCoreExiSlotControls').hide();
        });

        if (_staticPositioning) {
            $('#' + divId).draggable({
                stop: sCoreExi.onSlotChange,
                start: function (evt, ui) {

                    var zoom = sCoreExi.GetZoom();
                    var pointerY = (evt.pageY - $(sCoreExi.GetCanvas()).offset().top) / zoom - parseInt($(evt.target).css('top'));
                    var pointerX = (evt.pageX - $(sCoreExi.GetCanvas()).offset().left) / zoom - parseInt($(evt.target).css('left'));

                    sCoreExi.SetPointerX(pointerX);
                    sCoreExi.SetPointerY(pointerY);

                },
                drag: function (evt, ui) {

                    var canvasTop = $(sCoreExi.GetCanvas()).offset().top;
                    var canvasLeft = $(sCoreExi.GetCanvas()).offset().left;
                    var canvasHeight = $(sCoreExi.GetCanvas()).height();
                    var canvasWidth = $(sCoreExi.GetCanvas()).width();
                    var zoom = sCoreExi.GetZoom();

                    // Fix for zoom
                    ui.position.top = Math.round((evt.pageY - canvasTop) / zoom - sCoreExi.GetPointerY());
                    ui.position.left = Math.round((evt.pageX - canvasLeft) / zoom - sCoreExi.GetPointerX());

                    // Finally, make sure offset aligns with position
                    ui.offset.top = Math.round(ui.position.top + canvasTop);
                    ui.offset.left = Math.round(ui.position.left + canvasLeft);
                }
            });
        }
    }

    _pub.GetCanvas = function () {
        return _canvasContainer;
    }

    _pub.GetZoom = function () {
        return _zoomLevel;
    }

    _pub.GetPointerX = function () {
        return _pointerX;
    }

    _pub.GetPointerY = function () {
        return _pointerY;
    }

    _pub.SetPointerX = function (v) {
        _pointerX = v;
    }

    _pub.SetPointerY = function (v) {
        _pointerY = v;
    }

    _pub.removeSlot = function (src) {
        var domId = $(src).closest('.sCoreExiPortlet').attr('id');
        var id = domId.replace('sCoreExiSlot_', '');

        $('#' + domId).fadeOut(500, function () { $(this).remove(); });

        $.getJSON(_actionRoot + 'ExiRemoveSlot/?id=' + id, function (data) {
            sCoreExi.refreshCanvasData();
        });
    }
    
    _pub.renameSlot = function (src) {
        var domId = $(src).closest('.sCoreExiPortlet').attr('id');
        var id = domId.replace('sCoreExiSlot_', '');
        var currentName = $($(src).closest('.sCoreExiPortlet').find('.sCoreExiHeader').find('div')[0]).html();

        var name = prompt("Enter a new name for this item",currentName);

        if (name != null) {

            $($(src).closest('.sCoreExiPortlet').find('.sCoreExiHeader').find('div')[0]).html(name);

            $.getJSON(_actionRoot + 'ExiRenameSlot/?id=' + id + '&newName=' + name, function (data) {
                sCoreExi.refreshCanvasData();

            });
        }
    }



    _pub.togglePrintSlot = function (src) {
        var domId = $(src).closest('.sCoreExiPortlet').attr('id');
        var id = domId.replace('sCoreExiSlot_', '');

        if ($('#' + domId).hasClass('sCoreExiPortletPrint')) {
            $('#' + domId).removeClass('sCoreExiPortletPrint')
            $('#' + domId).find('.sCoreExiControlPrintSlot').find('.ui-button-text').html('&nbsp;Print');
        }
        else {
            $('#' + domId).addClass('sCoreExiPortletPrint')
            $('#' + domId).find('.sCoreExiControlPrintSlot').find('.ui-button-text').html('&nbsp;Clear');
        }

        if ($('.sCoreExiPortletPrint').length > 0) {
            $('.sCoreExiSectionSectionManagement').hide();
            $('.sCoreExiSectionPrintManagement').show();
        }
        else {
            $('.sCoreExiSectionSectionManagement').show();
            $('.sCoreExiSectionPrintManagement').hide();
        }
    }

    _pub.clearSelectedPrintSlots = function (src) {
        $('.sCoreExiPortletPrint').removeClass('sCoreExiPortletPrint');
        $('.sCoreExiSectionSectionManagement').show();
        $('.sCoreExiSectionPrintManagement').hide();
        $('.sCoreExiControlPrintSlot').find('.ui-button-text').html('&nbsp;Print');
    }

    _pub.openPrintMode = function () {
   
        var printContainer = $('<div />')
        .addClass('sCoreExiTmpPrintContainer')
        .css('width','800px');


        var controlContainer = $('<div />');
        controlContainer.addClass('sCoreExiControlContainer');
        controlContainer.addClass('sCoreExiNoPrint');
        $(controlContainer).appendTo(printContainer);

        var controlPanel = $('<div />');
        controlPanel.addClass('sCoreExiControlPanel');
        $(controlPanel).appendTo(controlContainer);

        var printButton = $('<button />');
        printButton.html('&nbsp;Print');
        printButton.css('font-size', '12px');
        printButton.appendTo($(controlPanel));
        printButton.addClass('');
        printButton.click(function () { window.print() });

        $(printButton).button({
            icons: { primary: "ui-icon-print" }
        })


        var closeButton = $('<button />');
        closeButton.html('&nbsp;Exit');
        closeButton.css('font-size', '12px');
        closeButton.appendTo($(controlPanel));
        closeButton.addClass('');
        closeButton.click(function () { sCoreExi.closePrintMode() });

        $(closeButton).button({
            icons: { primary: "ui-icon-close" }
        })

        $('<h1 />').html($(".sCoreExiSectionSelector option:selected").html()).appendTo($(printContainer));

        var printColumnA = $('<div />')
            .addClass('sCoreExiColumn')
            .css('width', '50%')
            .appendTo(printContainer);


        $(printColumnA).sortable({
            connectWith: ".sCoreExiColumn",
            handle: ".sCoreExiHeader",
            placeholder: "sCoreExiPlaceHolder ui-corner-all"
        });

        var printColumnB = $('<div />')
            .addClass('sCoreExiColumn')
            .css('width', '50%')
            .appendTo(printContainer);


        $(printColumnB).sortable({
            connectWith: ".sCoreExiColumn",
            handle: ".sCoreExiHeader",
            placeholder: "sCoreExiPlaceHolder ui-corner-all"
        });

        var currentColumn = 0;

        var printSlots = $('.sCoreExiPortletPrint');

        for (var ii = 0; ii < printSlots.length ; ii++) {

            var printColumn = null;

            if (currentColumn == 1) {
                currentColumn = 0;
                printColumn = printColumnB;
            }
            else {
                currentColumn = 1;
                printColumn = printColumnA;
            }

            var cloneContainer = $('<div />');
            cloneContainer.css('padding', '10px');
            cloneContainer.css('page-break-inside', 'avoid');
            cloneContainer.append($(printSlots[ii]).find('.sCoreExiHeader').clone());
            cloneContainer.append($(printSlots[ii]).find('.sCoreExiContent').clone());

            $(printColumn).append(cloneContainer);
        }


        $('body').children().addClass('sCoreExiNonPrintable');

        $('body').addClass('sCoreExiPrintBody');

        $(printContainer).appendTo($('body'));

    }


    _pub.closePrintMode = function()
    {
        $('.sCoreExiTmpPrintContainer').remove();
        $('.sCoreExiNonPrintable').removeClass('sCoreExiNonPrintable');
        sCoreExi.clearSelectedPrintSlots();


        $('body').removeClass('sCoreExiPrintBody');

        /* Cloning stuff makes things act funny. We just redraw the active section to solve this */
        sCoreExi.renderActiveSection();
    }

    _pub.onSlotChange = function (event, ui) {
        
        var element = ui.helper;

        if (element == 'undefined') {
            element = ui.item.parent()[0];
        }

        if (element) {

            var domId = $(element).attr('id')
            var id = domId.replace('sCoreExiSlot_', '');

            var columns = $(_canvasContainer).find('.sCoreExiColumn');
            var currentColumn = 0;
            var currentPosition = 0;

            /* Find out the index of the column the slot currently belongs to */

            for (var i = 0; i < columns.length; i++) {
                var match = $(columns[i]).find('#' + domId);

                if (match.length > 0) {
                    currentColumn = i;
                }

            };


            var newX = ui.position.left;
            var newY = ui.position.top;

            if (newX < 1) {
                newX = 1;
            }

            if (newY < 1) {
                newY = 1;
            }

            /* update the server with new position and then reload data */

            $.getJSON(_actionRoot + 'ExiMoveSlot/?id=' + id + '&newPosition=' + currentPosition + '&newColumn=' + currentColumn + '&newX=' + newX + '&newY=' + newY, function (data) {
                sCoreExi.refreshCanvasData();
            });

        }

    }


    _pub.addSharedSection = function () {

        var name = prompt("Please enter a name for the new shared section");

        if (name != null) {
            $.getJSON(_actionRoot + 'ExiCreateSection/?name=' + name + '&isShared=true&columns=3', function (data) {
                sCoreExi.refreshCanvasData();
            });
        }

    }

    _pub.renameSharedSection = function () {

        var activeSection = _canvasData["Sections"][_activeSectionIndex];

        var name = prompt("Please enter a new name for this section");

        if (name != null) {
            $.getJSON(_actionRoot + 'ExiRenameSection/?newName=' + name + '&id=' + activeSection['Identifier'], function (data) {
                sCoreExi.refreshCanvasData();
            });
        }

    }

    _pub.setDefaultSection = function () {

        var activeSection = _canvasData["Sections"][_activeSectionIndex];

        $.getJSON(_actionRoot + 'ExiSetDefaultSection/?id=' + activeSection['Identifier'], function (data) {
            alert('Default section has been set');
        });

    }


    _pub.disableEditingCanvas = function () {

        if (this._staticPositioning) {
            $('.sCoreExiPortlet').draggable("disable");
        }
        else {
            $(".sCoreExiColumn").sortable("disable");
        }
  
        $(".sCoreExiControlAddWidget").hide();
        $('.sCoreExiControlRemoveSlot').hide();
        $('.sCoreExiHeader').css('cursor', 'auto');
        $('.sCoreExiControlSectionRename').hide();
        $('.sCoreExiControlEditSlotTitle').hide();
    }

    _pub.enableEditingCanvas = function () {

        if (_staticPositioning) {
        }
        else {

            var handle = '.sCoreExiHeader';

            if (_nonTileMode) {
                handle = '.sCoreExiContent';
            }

            $(".sCoreExiColumn").sortable({
                connectWith: ".sCoreExiColumn",
                handle: handle,
                placeholder: "sCoreExiPlaceHolder ui-corner-all",
                update: sCoreExi.onSlotChange
            });
        }

        $('.sCoreExiControlRemoveSlot').show();
        $(".sCoreExiControlAddWidget").show();
        $('.sCoreExiHeader').css('cursor', 'pointer');
        $('.sCoreExiControlSectionRename').show();
        $('.sCoreExiControlEditSlotTitle').show();
    }

    /********************************************/
    /******* Wizard Functionaility **************/
    /********************************************/

    _pub.buildWizard = function() {
       
        $('#sCoreExiWizardDialog').remove();

        $.get(_actionRoot + 'ExiGetWizardTemplate', function (data) {
            sCoreExi.renderWizard(data);
        });
        
    }

    _pub.renderWizard = function (html) {

        var element = $('<div />');
        $(element).attr('id', 'sCoreExiWizardDialog');
        $(element).css('display', 'none');

        $(element).html($(html));
        $(element).appendTo($('body'));

        $('#sCoreExiWizard').smartWizard({
            // Properties
            selected: 0,  // Selected Step, 0 = first step   
            keyNavigation: true, // Enable/Disable key navigation(left and right keys are used if enabled)
            enableAllSteps: false,  // Enable/Disable all steps on first load
            transitionEffect: 'fade', // Effect on navigation, none/fade/slide/slideleft
            contentURL: null, // specifying content url enables ajax content loading
            contentURLData: null, // override ajax query parameters
            contentCache: true, // cache step contents, if false content is fetched always from ajax url
            cycleSteps: false, // cycle step navigation
            enableFinishButton: false, // makes finish button enabled always
            hideButtonsOnDisabled: false, // when the previous/next/finish buttons are disabled, hide them instead
            errorSteps: [],    // array of step numbers to highlighting as error steps
            labelNext: 'Next', // label for Next button
            labelPrevious: 'Previous', // label for Previous button
            labelFinish: 'Add',  // label for Finish button        
            noForwardJumping: true,
            onLeaveStep: sCoreExi.wizardLeaveStep, // triggers when leaving a step
            onShowStep: sCoreExi.wizardShowStep,  // triggers when showing a step
            onFinish: sCoreExi.wizardFinish,  // triggers when Finish button is clicked
            includeFinishButton: true   // Add the finish button
        });


        var step1Container = $("#sCoreExiWizardDialog").find('#step-1-choices').find('div').first();
        step1Container.find('.sCoreExiWizardChoice').remove();

        for (i = 0; i < _wizardData.length; i++) {

            var callback = new Function('sCoreExi.wizardSelectWidgetGroup(' + i + ')');
            var choice = _wizardData[i];
            var element = sCoreExi.wizardRenderChoice(choice['Name'], 'step-1-option-group', callback);
            $(element).appendTo(step1Container);
        }

    }

    _pub.showWizard = function () {
        $('#sCoreExiWizardDialog').dialog({
            height: 570,
            width: 1050,
            modal: true,
            title: 'Add Item',
            resizable: false,
            open: function (event, ui) {
                $('.ui-dialog').css('z-index', '99999');
                $('.ui-dialog').css('transform', 'scale(1)');
                $('.ui-dialog').css('box-shadow', '10px 10px 5px #888888');
            },
            close: sCoreExi.buildWizard
        });

       
        
    }

    _pub.wizardLeaveStep = function (obj, context) {

        if (context.toStep == 2) {
            
            if (_wizardSelectedWidgetGroupIndex == null) {
                alert('You must select a data source');
                return false;
            }

        }

        if (context.toStep == 3) {

            if (_wizardSelectedWidgetIndex == null) {
                alert('You must select a filter');
                return false;
            }

        }

        if (context.toStep == 4) {

            var optionElements = $('#sCoreExiWizardDialog').find('.sCoreExiWizardWidgetOption');

            for (i = 0; i < optionElements.length; i++) {

                if ($(optionElements[i]).data('Required') == true) {
                    if($(optionElements[i]).val() == '')
                    {
                        alert('You must specify a value for ' + $(optionElements[i]).data('Label'));
                        return false;
                    }
                }

            }

        }

        return true;
    }

    _pub.wizardShowStep = function (obj, context) {
        if(context.toStep < context.fromStep)
        {
            sCoreExi.wizardResetTo(context.toStep);
        }
    }

    _pub.wizardFinish = function (obj, context) {

        if (_wizardSelectedRendererIndex == null) {
            alert('You must select a view');
            return false;
        }

        var selectedWidget = _wizardData[_wizardSelectedWidgetGroupIndex].Widgets[_wizardSelectedWidgetIndex]
        var dataSourceTypeName = selectedWidget['DataSourceTypeName'];

        var selectedRenderer = selectedWidget.RendererOptions[_wizardSelectedRendererIndex];
        var rendererTypeName = selectedRenderer['TypeName'];

        var optionsQuery = '';

        /* Cycle through all options and build the required options dictionary */

        var optionElements = $('#sCoreExiWizardDialog').find('.sCoreExiWizardWidgetOption');

        for (i = 0; i < optionElements.length; i++) {

            if (optionsQuery != '') {
                optionsQuery = optionsQuery + ',';
            }

            optionsQuery = optionsQuery + $(optionElements[i]).data('Key');
            optionsQuery = optionsQuery + ':';
            optionsQuery = optionsQuery + $(optionElements[i]).val();
        }

        var descriptionContainer = $("#sCoreExiWizardDialog").find('#step-4-description').html('Loading preview...');

        var activeSection = _canvasData["Sections"][_activeSectionIndex];

        /* render the preview */
        var queryBuilder = _actionRoot + 'ExiCreateSlot/';
        queryBuilder = queryBuilder + '?dataSourceTypeName=' + encodeURIComponent(dataSourceTypeName);
        queryBuilder = queryBuilder + '&rendererTypeName=' + encodeURIComponent(rendererTypeName);
        queryBuilder = queryBuilder + '&options=' + encodeURIComponent(optionsQuery);
        queryBuilder = queryBuilder + '&sectionId=' + activeSection["Identifier"];
        queryBuilder = queryBuilder + '&column=' + 0;
        queryBuilder = queryBuilder + '&position=' + 0;
        queryBuilder = queryBuilder + '&X=' + 0;
        queryBuilder = queryBuilder + '&Y=' + 0;

        $('#sCoreExiWizardDialog').dialog('close');

        $.getJSON(queryBuilder, function (data) {
            sCoreExi.createSlot(data, 0);
            sCoreExi.loadSlotContent(data['Identifier'], false, false);
        });

        sCoreExi.refreshCanvasData();

        return true;
    }

    _pub.wizardResetTo = function(step)
    {
        $("#sCoreExiWizard").smartWizard('enableFinish', false);

        if (step < 2) {
            _wizardSelectedWidgetGroupIndex = null;
            _wizardSelectedWidgetIndex = null;
            $("#sCoreExiWizard").smartWizard('disableStep', 2);

            var step2Container = $("#sCoreExiWizardDialog").find('#step-2-choices').find('div').first();
            step2Container.find('.sCoreExiWizardChoice').remove();
            $("#sCoreExiWizardDialog").find('#step-2-description').html('');
            
            $("#sCoreExiWizardDialog").find('#step-1-description').html('');
            $("#sCoreExiWizardDialog").find('#step-1-choices').find('input').prop('checked', false);
        }

        if (step < 3) {
            _wizardSelectedWidgetIndex = null;
            $("#sCoreExiWizard").smartWizard('disableStep', 3);

            var step3Container = $("#sCoreExiWizardDialog").find('#step-3-choices').find('div').first();
            step3Container.find('.sCoreExiWizardWidgetOptionContainer').remove();
            $("#sCoreExiWizardDialog").find('#step-3-description').html('');

            $("#sCoreExiWizardDialog").find('#step-2-description').html('');
            $("#sCoreExiWizardDialog").find('#step-2-choices').find('input').prop('checked', false);

            var step4Container = $("#sCoreExiWizardDialog").find('#step-4-choices').find('div').first();
            step4Container.find('.sCoreExiWizardChoice').remove();
            $("#sCoreExiWizardDialog").find('#step-4-description').html('');

        }

        if (step < 4) {
            _wizardSelectedRendererIndex = null;
            $("#sCoreExiWizard").smartWizard('disableStep', 4);

            $("#sCoreExiWizardDialog").find('#step-4-choices').find('input').prop('checked', false);
            $("#sCoreExiWizardDialog").find('#step-4-description').html('');
        }
    }

    _pub.wizardSelectWidgetGroup = function(val)
    {

        var descriptionContainer = $('<div />');
        descriptionContainer.css('background-color', '#f5f5f5');
        descriptionContainer.css('border', 'solid 1px #ada29f');
        descriptionContainer.css('padding', '5px');
        descriptionContainer.css('box-shadow', '5px 5px 2px #888888');
        descriptionContainer.html(_wizardData[val]["Description"]);

        $("#sCoreExiWizardDialog").find('#step-1-description').html(descriptionContainer);

        /* Assign selection */
        _wizardSelectedWidgetGroupIndex = val;

        /* Render 2nd step options */
        var step2Container = $("#sCoreExiWizardDialog").find('#step-2-choices').find('div').first();
        step2Container.find('.sCoreExiWizardChoice').remove();

        for (i = 0; i < _wizardData[val].Widgets.length; i++) {

            var callback = new Function('sCoreExi.wizardSelectWidget(' + i + ')');
            var choice = _wizardData[val].Widgets[i];
            var element = sCoreExi.wizardRenderChoice(choice['Name'], 'step-2-option-group', callback);
            $(element).appendTo(step2Container);
        }

    }

    _pub.wizardSelectWidget = function (val) {

        var descriptionContainer = $('<div />');
        descriptionContainer.css('background-color', '#f5f5f5');
        descriptionContainer.css('border', 'solid 1px #ada29f');
        descriptionContainer.css('padding', '5px');
        descriptionContainer.css('box-shadow', '5px 5px 2px #888888');
        descriptionContainer.html(_wizardData[_wizardSelectedWidgetGroupIndex].Widgets[val]["Description"]);

        $("#sCoreExiWizardDialog").find('#step-2-description').html(descriptionContainer);

        _wizardSelectedWidgetIndex = val;


        /* Build Renderers screen */
        var step4Container = $("#sCoreExiWizardDialog").find('#step-4-choices').find('div').first();
        step4Container.find('.sCoreExiWizardChoice').remove();

        for (i = 0; i < _wizardData[_wizardSelectedWidgetGroupIndex].Widgets[val].RendererOptions.length; i++) {

            var callback = new Function('sCoreExi.wizardSelectRenderer(' + i + ')');
            var choice = _wizardData[_wizardSelectedWidgetGroupIndex].Widgets[val].RendererOptions[i];
            var element = sCoreExi.wizardRenderChoice(choice['Name'], 'step-4-option-group', callback);
            $(element).appendTo(step4Container);
        }


        /* Build Widget Options sreen */

        var step3Container = $("#sCoreExiWizardDialog").find('#step-3-choices').find('div').first();
        step3Container.find('.sCoreExiWizardWidgetOptionContainer').remove();

        for (ii = 0; ii < _wizardData[_wizardSelectedWidgetGroupIndex].Widgets[val].Options.length; ii++) {
            var option = _wizardData[_wizardSelectedWidgetGroupIndex].Widgets[val].Options[ii];
            var element = sCoreExi.wizardRenderOption(option);
            $(element).appendTo(step3Container);
        };

    }

    _pub.wizardSelectRenderer = function (val) {

        _wizardSelectedRendererIndex = val;

        var selectedWidget = _wizardData[_wizardSelectedWidgetGroupIndex].Widgets[_wizardSelectedWidgetIndex]
        var dataSourceTypeName = selectedWidget['DataSourceTypeName'];

        var selectedRenderer = selectedWidget.RendererOptions[val];
        var rendererTypeName = selectedRenderer['TypeName'];

        var optionsQuery = '';

        /* Cycle through all options and build the required options dictionary */

        var optionElements = $('#sCoreExiWizardDialog').find('.sCoreExiWizardWidgetOption');

        for (i = 0; i < optionElements.length; i++) {

            if (optionsQuery != '') {
                optionsQuery = optionsQuery + ',';
            }

            optionsQuery = optionsQuery + $(optionElements[i]).data('Key');
            optionsQuery = optionsQuery + ':';
            optionsQuery = optionsQuery + $(optionElements[i]).val();
        }

        var descriptionContainer = $("#sCoreExiWizardDialog").find('#step-4-description').html('Loading preview...');

        /* render the preview */
        var queryBuilder = _actionRoot + 'ExiRenderPreview/';
        queryBuilder = queryBuilder + '?dataSourceTypeName=' + encodeURIComponent(dataSourceTypeName);
        queryBuilder = queryBuilder + '&rendererTypeName=' + encodeURIComponent(rendererTypeName);
        queryBuilder = queryBuilder + '&options=' + encodeURIComponent(optionsQuery);

        descriptionContainer.load(queryBuilder);
            
    }
 
    _pub.wizardRenderChoice = function (labelText, group, callback) {
        var container = $('<div />');
        container.css('padding-bottom', '5px');
        container.addClass('sCoreExiWizardChoice');

        var radio = $('<input />');
        $(radio).attr('type', 'radio');
        $(radio).attr('name', group);
        $(radio).css('position', 'relative');
        $(radio).css('z-index', '10000');
        $(radio).click(callback);

        var label = $('<span />');
        $(label).html(labelText);

        $(radio).appendTo(container);
        $(label).appendTo(container);

        return container;
    }

    /********************************************************/
    /******* Wizard Choice & Option Rendering **************/
    /*******************************************************/

    _pub.wizardRenderOption = function(opt)
    {
        var element = [];

        if (opt['Hidden'] == true) {
            element = sCoreExi.wizardRenderOptionHidden(opt);
        }
        else if (opt['SelectList'] != null) {
            element = sCoreExi.wizardRenderOptionSelectList(opt);
        }
        else {
            element = sCoreExi.wizardRenderOptionTextBox(opt);
        }

        var elementContainer = $('<div />');
        $(elementContainer).addClass('sCoreExiWizardWidgetOptionContainer');

        if (opt['Hidden'] == false) {
            var label = $('<div / >');
            $(label).html(opt['Label']);
            $(label).css('font-weight', 'bold');
            $(label).css('padding-top', '5px');
            $(label).appendTo(elementContainer);
        }

        $(element).appendTo(elementContainer);
        
        return $(elementContainer);
    }

    _pub.wizardRenderOptionTextBox = function (opt) {
        var element = $('<input type="text" />');
        $(element).data('Key', opt['Key']);
        $(element).data('Label', opt['Label']);
        $(element).data('Required', opt['Required']);
        $(element).addClass('sCoreExiWizardWidgetOption');
        $(element).css('position', 'relative');
        $(element).css('z-index', '10000');
        return $(element);
    }

    _pub.wizardRenderOptionSelectList = function (opt) {
        var element = $('<select />');
        $(element).data('Key', opt['Key']);
        $(element).data('Label', opt['Label']);
        $(element).data('Required', opt['Required']);
        $(element).addClass('sCoreExiWizardWidgetOption');
        $(element).css('position', 'relative');
        $(element).css('z-index', '10000');

        for (i = 0; i < opt['SelectList'].length; i++) {
            var option = $('<option />');
            $(option).attr('Value', opt['SelectList'][i]['Key']);
            $(option).html(opt['SelectList'][i]['Value']);
            $(option).appendTo($(element));
        }

        return $(element);
    }

    _pub.wizardRenderOptionHidden = function (opt) {
        var element = $('<input type="hidden" />');
        $(element).data('Key', opt['Key']);
        $(element).data('Required', opt['Required']);
        $(element).addClass('sCoreExiWizardWidgetOption');
        $(element).val(opt['DefaultValue']);
        return $(element);
    }

    _pub.sendKioskCommand = function (command) {

        console.log('sCore.Exi.Kiosk.Command.' + command);
    }

    return _pub;
}();