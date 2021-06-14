adminApp.controller("batchOperationController", ['$scope', 'NgTableParams', '$http', '$element', function ($scope, NgTableParams, $http, $element) {
    $scope.selectedFieldItem = null;
    $scope.selectedCustomizeItem = null;
    $scope.selectedFieldList = new Array();
    $scope.isAllActive = false;
    $scope.IsValueOnly = 0;
    $scope.changeFieldList2 = null;
    $scope.changeFieldList = null;
    $scope.isAllActiveRight = false;
    $scope.batchFieldType = {
        _Varchar: 0,
        _Integer: 1,
        _Double: 2,
        _DateTime: 3
    };

    $scope.loadFieldData = function () {
        $scope.selectedFieldList = new Array();
        $scope.selectedFieldItem = null;
        $scope.tableName = $("[name='customRadioAlt']:checked").val();
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/BatchOperations/GetFieldList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { tableName: $scope.tableName, type: false }
        }).then(function (response) {
            fireCustomLoading(false);
            $scope.fieldList = response.data;
        });
    };

    $scope.loadChangeFieldData = function () {
        $scope.selectedChangeFieldList = new Array();
        $scope.selectedChangeFieldItem = null;
        $scope.tableName = $("[name='customRadioAlt']:checked").val();
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/BatchOperations/GetFieldList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { tableName: $scope.tableName, type: true }
        }).then(function (response) {
            fireCustomLoading(false);
            $scope.changeFieldList = response.data;
            $scope.changeFieldList2 = angular.copy($scope.changeFieldList);
        });
    };

    $scope.setSelectedField = function (item) {
        $scope.selectedFieldItem = item;
    };

    $scope.setChangeSelectedField = function (item) {
        $scope.selectedChangeFieldItem = item;
    };

    $scope.upOrDownItem = function (value) {
        if ($scope.selectedCustomizeItem === null)
            return;

        $scope.selectedFieldList = $scope.selectedFieldList.sort(function (a, b) {
            a = parseInt(a['PrioritySelected']);
            b = parseInt(b['PrioritySelected']);
            return a - b;
        });

        var index = $scope.selectedFieldList.indexOf($scope.selectedCustomizeItem);
        if (index != 0 && value < 0) {
            var beforeValue = $scope.selectedFieldList[index - 1].PrioritySelected;
            $scope.selectedFieldList[index - 1].PrioritySelected = $scope.selectedFieldList[index].PrioritySelected;
            $scope.selectedFieldList[index].PrioritySelected = beforeValue;
        }
        else if (index <= $scope.selectedFieldList.length - 1 && value > 0) {
            var beforeValue = $scope.selectedFieldList[index + 1].PrioritySelected;
            $scope.selectedFieldList[index + 1].PrioritySelected = $scope.selectedFieldList[index].PrioritySelected;
            $scope.selectedFieldList[index].PrioritySelected = beforeValue;
        }
    };

    $scope.selectedFieldCustomize = function (item) {
        $scope.selectedCustomizeItem = item;

        if (item.IsSave) {
            $scope.fieldValue = item.FieldValue;
            var index = item.IsEqual === "true" ? 0 : 1;
            $("[name='isEqual']:input")[index].checked = true;
            index = item.ProcessType === 'AND' ? 0 : 1;
            $("[name='processType']:input")[index].checked = true;
            $("[name='processSelection']").val(item.Process);
            $('#dateStartValue').val(item.DateStartValue);
            $('#dateEndValue').val(item.DateEndValue);
        }
        else
            $scope.clearTextValues();

        if (item.Type === $scope.batchFieldType._Integer || item.Type === $scope.batchFieldType._Double) {
            $scope.isNumericSelected = true;
            $scope.dateActive = false;
            firePriceFormat();
        }
        else if (item.Type === $scope.batchFieldType._Varchar) {
            $scope.isNumericSelected = false;
            $scope.dateActive = false;
            destroyPriceFormat();
        }
        else
            if (item.Type === $scope.batchFieldType._DateTime) {
                $scope.dateActive = true;
            }

        if (item.FieldText === "(" || item.FieldText === ")") {
            $scope.isAllActive = true;
            if (item.FieldText === "(") {
                $scope.isAllActiveRight = true;
            }
            else {
                $scope.isAllActiveRight = false;
            }
        }
        else {
            $scope.isAllActive = false;
            $scope.isAllActiveRight = false;
        }
    };

    $scope.selectedChangeFieldCustomize = function (item) {
        $scope.selectedChangeCustomizeItem = item;

        if (item.IsSave) {
            $scope.fieldValueChange = item.FieldValue;
            $("[name='changeType']:input")[item.ChangeType].checked = true;
            $scope.IsValueOnly = item.ChangeType;
            $("[name='fieldSelection']").val(item.FieldSelection);
            $('#dateStartValueChange').val(item.DateStartValue);
            $('#dateEndValueChange').val(item.DateEndValue);
        }
        else
            $scope.clearChangeTextValues();

        if (item.Type === $scope.batchFieldType._Integer || item.Type === $scope.batchFieldType._Double) {
            $scope.dateActiveChange = false;
            firePriceFormat();
        }
        else if (item.Type === $scope.batchFieldType._Varchar) {
            $scope.dateActiveChange = false;
            destroyPriceFormat();
        }
        else
            if (item.Type === $scope.batchFieldType._DateTime) {
                $scope.dateActiveChange = true;
            }

        //if (item.FieldText === "(" || item.FieldText === ")")
        //    $scope.isAllActive = true;
        //else
        //    $scope.isAllActive = false;
    };

    $scope.addBrackets = function (value) {
        $scope.selectedFieldList = $scope.selectedFieldList.sort(function (a, b) {
            a = parseInt(a['PrioritySelected']);
            b = parseInt(b['PrioritySelected']);
            return a - b;
        });

        $scope.selectedFieldList.push(
            {
                FieldName: value,
                FieldText: value,
                Explanation: '',
                PrioritySelected: 0,
                Type: $scope.batchFieldType._Varchar,
                IsSave: value === '(' ? true : false
            }
        );

        if ($scope.selectedFieldList.length >= 2)
            $scope.selectedFieldList[$scope.selectedFieldList.length - 1].PrioritySelected = $scope.selectedFieldList[$scope.selectedFieldList.length - 2].PrioritySelected + 1;
        else
            $scope.selectedFieldList[$scope.selectedFieldList.length - 1].PrioritySelected = 0;
    };

    $scope.checkSelectedFieldList = function () {
        var _result = true;
        var _continue = true;
        angular.forEach($scope.selectedFieldList, function (value) {
            if (_continue && row.IsSave === false) {
                _result = false;
                _continue = false;
            }
        });

        return _result;
    };

    $scope.addSelectedField = function () {
        if ($scope.selectedFieldItem === null)
            return;

        var index = $scope.fieldList.indexOf($scope.selectedFieldItem);
        $scope.fieldList.splice(index, 1);
        $scope.selectedFieldList.push($scope.selectedFieldItem);
        if ($scope.selectedFieldList.length >= 2)
            $scope.selectedFieldList[$scope.selectedFieldList.length - 1].PrioritySelected = $scope.selectedFieldList[$scope.selectedFieldList.length - 2].PrioritySelected + 1;
        else
            $scope.selectedFieldList[$scope.selectedFieldList.length - 1].PrioritySelected = 0;

        $scope.selectedFieldItem = null;
    };

    $scope.addSelectedChangeField = function () {
        if ($scope.selectedChangeFieldItem === null)
            return;

        var index = $scope.changeFieldList.indexOf($scope.selectedChangeFieldItem);
        $scope.changeFieldList.splice(index, 1);
        $scope.selectedChangeFieldList.push($scope.selectedChangeFieldItem);
        if ($scope.selectedChangeFieldList.length >= 2)
            $scope.selectedChangeFieldList[$scope.selectedChangeFieldList.length - 1].PrioritySelected = $scope.selectedChangeFieldList[$scope.selectedChangeFieldList.length - 2].PrioritySelected + 1;
        else
            $scope.selectedChangeFieldList[$scope.selectedChangeFieldList.length - 1].PrioritySelected = 0;

        $scope.selectedChangeFieldItem = null;
    };

    $scope.removeSelectedField = function () {
        if ($scope.selectedCustomizeItem === null)
            return;

        var index = $scope.selectedFieldList.indexOf($scope.selectedCustomizeItem);
        $scope.selectedFieldList.splice(index, 1);
        if ($scope.selectedCustomizeItem.FieldName != "(" && $scope.selectedCustomizeItem.FieldName != ")")
            $scope.fieldList.push($scope.selectedCustomizeItem);

        $scope.selectedCustomizeItem = null;
    };

    $scope.removeSelectedChangeField = function () {
        if ($scope.selectedChangeCustomizeItem === null)
            return;

        var index = $scope.selectedChangeFieldList.indexOf($scope.selectedChangeCustomizeItem);
        $scope.selectedChangeFieldList.splice(index, 1);
        $scope.changeFieldList.push($scope.selectedChangeCustomizeItem);
        $scope.selectedChangeCustomizeItem = null;
    };

    $scope.setFieldValues = function () {
        var index = $scope.selectedFieldList.indexOf($scope.selectedCustomizeItem);

        $scope.selectedFieldList[index].FieldValue = $scope.fieldValue;
        $scope.selectedFieldList[index].IsEqual = $("[name='isEqual']:checked").val();
        $scope.selectedFieldList[index].ProcessType = $("[name='processType']:checked").val();
        $scope.selectedFieldList[index].Process = $("[name='processSelection']").val();
        $scope.selectedFieldList[index].DateStartValue = $('#dateStartValue').val();
        $scope.selectedFieldList[index].DateEndValue = $('#dateEndValue').val();
        $scope.selectedFieldList[index].IsSave = true;
    };

    $scope.setChangeFieldValues = function () {
        var index = $scope.selectedChangeFieldList.indexOf($scope.selectedChangeCustomizeItem);

        $scope.selectedChangeFieldList[index].FieldValue = $scope.fieldValueChange;
        $scope.selectedChangeFieldList[index].ChangeType = $("[name='changeType']:checked").val();
        $scope.selectedChangeFieldList[index].FieldSelection = $("[name='fieldSelection']").val();
        $scope.selectedChangeFieldList[index].DateStartValue = $('#dateStartValueChange').val();
        $scope.selectedChangeFieldList[index].DateEndValue = $('#dateEndValueChange').val();
        $scope.selectedChangeFieldList[index].IsSave = true;
    };

    $scope.clearTextValues = function () {
        $scope.fieldValue = '';
        $("[name='isEqual']:input")[0].checked = true;
        $("[name='processType']:input")[0].checked = true;
        $("[name='processSelection']").val('=');
        $('#dateStartValue').val('');
        $('#dateEndValue').val('');
    };

    $scope.clearChangeTextValues = function () {
        $scope.fieldValueChange = '';
        $("[name='changeType']:input")[0].checked = true;
        $("[name='fieldSelection'] option:eq(0)").prop('selected', true);
        $('#dateStartValueChange').val('');
        $('#dateEndValueChange').val('');
        $scope.IsValueOnly = 0;
    };

    $scope.generateQuery = function () {
        $scope.tableName = $("[name='customRadioAlt']:checked").val();
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/BatchOperations/GenerateQuery",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { fieldList: $scope.selectedFieldList, changeFieldList: $scope.selectedChangeFieldList, table: $scope.tableName }
        }).then(function (response) {
            fireCustomLoading(false);
            var retVal = jQuery.parseJSON(response.data);
            $scope.QueryText = retVal.query;
        });
    };

    $(window).load(function () {
        $('#rootwizard').bootstrapWizard({
            onTabShow: function (tab, navigation, index) {
                var $total = navigation.find('li').length;
                var $current = index + 1;

                // If it's the last tab then hide the last button and show the finish instead
                if ($current >= $total) {
                    $('#rootwizard').find('.pager .next').hide();
                    $('#rootwizard').find('.pager .finish').show();
                    $('#rootwizard').find('.pager .finish').removeClass('disabled');
                }
                else {
                    $('#rootwizard').find('.pager .next').show();
                    $('#rootwizard').find('.pager .finish').hide();
                }
            },
            onNext: function (tab, navigation, index) {
                var form = $('form[name="step' + index + '"]');

                if (index === 1) {
                    angular.element(document.getElementById('divBatch')).scope().loadFieldData();
                }
                else if (index === 2) {
                    angular.element(document.getElementById('divBatch')).scope().loadChangeFieldData();
                    return angular.element(document.getElementById('divBatch')).scope().checkSelectedFieldList();
                }
                else if (index === 3) {
                    angular.element(document.getElementById('divBatch')).scope().generateQuery();
                }

                form.parsley().validate();

                if (!form.parsley().isValid()) {
                    return false;
                }
            },
            onTabClick: function (tab, navigation, index) {
                var form = $('form[name="step' + (index + 1) + '"]');
                form.parsley().validate();

                if (!form.parsley().isValid()) {
                    return false;
                }
            }
        });
    });

    $(document).ready(function () { });
}]);