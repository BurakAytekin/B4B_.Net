adminApp.controller('TaskListController', function ($scope, $http) {

    // #region Veriables
    $scope.taskList = {};
    $scope.taskListComment = {};
    $scope.selectedTask = -1;

    $scope.searchCriteria = {
        StartDate: '',
        EndDate: '',
        GeneralSearchText: '',
        Statu: -1
    };

    // #endregion

    $scope.openModal = function (id) {
        $scope.selectedTask = id;
        $scope.getTaskListComment(id);
        $('#mTaskListComment').appendTo("body").modal('show');

    };
    $scope.getTaskList = function () {

        $scope.searchCriteria.StartDate = $('#iTaskListStartDate').val();
        $scope.searchCriteria.EndDate = $('#iTaskListEndDate').val();


        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/TaskList/GetTaskList",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: {
                startDate: $scope.searchCriteria.StartDate,
                endDate: $scope.searchCriteria.EndDate,
                generalSearchText: $scope.searchCriteria.GeneralSearchText,
                statu: $scope.searchCriteria.Statu
            }

        }).then(function (response) {
            $scope.taskList = response.data;
            fireCustomLoading(false);
        });
    };
    $scope.getTaskListComment = function (id) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/TaskList/GetTaskListComment",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { taskListId: id }

        }).then(function (response) {
            $scope.taskListComment = response.data;
            fireCustomLoading(false);
        });
    };
    $scope.addTask = function () {
        var header = $('#txtHeader').val();
        var content = $('#txtContent').val();
        if (header === '' || content === '') {
            iziToast.error({
                //title: 'Hata',
                message: 'Lütfen ilgili alanları doldurunuz',
                position: 'topCenter'
            });
        } else {
            fireCustomLoading(true);
            $http({
                method: "POST",
                url: "/Admin/TaskList/AddTask",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: {
                    header: header,
                    content: content
                }

            }).then(function (response) {
                var retVal = jQuery.parseJSON(response.data);
                if (retVal.statu === "success") {
                    $('#txtHeader').val('');
                    $('#txtContent').val('');
                    $scope.getTaskList();
                    iziToast.success({
                        message: retVal.message,
                        position: 'topCenter'
                    });
                } else {
                    iziToast.error({
                        //title: 'Hata',
                        message: retVal.message,
                        position: 'topCenter'
                    });
                    fireCustomLoading(false);
                }
            });
        }

    };
    $scope.keypressEventAddTaskComment = function (e) {
        var key;
        if (window.event)
            key = window.event.keyCode; //IE
        else
            key = (e.which) ? e.which : event.keyCode; //Firefox
        if (key === 13) {
            $scope.addTaskComment();
        }
    };
    $scope.addTaskComment = function () {
        var id = $scope.selectedTask;
        var content = $('#txtCommentContent').val();
        if (header === '' || content === '') {
            iziToast.error({
                //title: 'Hata',
                message: 'Lütfen ilgili alanları doldurunuz',
                position: 'topCenter'
            });
        } else {
            fireCustomLoading(true);
            $http({
                method: "POST",
                url: "/Admin/TaskList/AddTaskComment",
                headers: { "Content-Type": "Application/json;charset=utf-8" },
                data: { taskListId: id, content: content }

            }).then(function (response) {
                fireCustomLoading(false);
                var retVal = jQuery.parseJSON(response.data);
                if (retVal.statu === "success") {
                    $('#txtCommentContent').val('');
                    $scope.getTaskListComment($scope.selectedTask);
                    iziToast.success({
                        message: retVal.message,
                        position: 'topCenter'
                    });
                } else {
                    iziToast.error({
                        //title: 'Hata',
                        message: retVal.message,
                        position: 'topCenter'
                    });

                }
            });
        }

    };
    $scope.changeTaskStatus = function (id, status) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/TaskList/ChangeTaskStatus",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: id, status: status }

        }).then(function (response) {
            var retVal = jQuery.parseJSON(response.data);
            if (retVal.statu === "success") {
                $scope.getTaskList();
                iziToast.success({
                    message: retVal.message,
                    position: 'topCenter'
                });
            } else {
                iziToast.error({
                    //title: 'Hata',
                    message: retVal.message,
                    position: 'topCenter'
                });
                fireCustomLoading(false);

            }
        });
    };
    $scope.askForDelete = function (item) {
        $.confirm({
            title: 'Uyarı!',
            content: "Silmek istediğinize emin misiniz?",
            buttons:
            {
                EVET: {
                    btnClass: 'btn-blue',
                    action: function () { $scope.deleteTask(item); }

                },
                HAYIR: {
                    btnClass: 'btn-red any-other-class',
                    action: function () {
                        iziToast.error({
                            message: 'Silme işleminiz iptal edilmiştir.',
                            position: 'topCenter'
                        });

                    }
                }

            }
        });
    };
    $scope.deleteTask = function (item) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/TaskList/DeleteTask",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: item.Id }

        }).then(function (response) {
            var retVal = jQuery.parseJSON(response.data);
            if (retVal.statu === "success") {
                var index = $scope.taskList.indexOf(item);
                $scope.taskList.splice(index, 1);
                iziToast.success({
                    message: retVal.message,
                    position: 'topCenter'
                });
            } else {
                iziToast.error({
                    //title: 'Hata',
                    message: retVal.message,
                    position: 'topCenter'
                });

            };
            fireCustomLoading(false);

        });
    };
    $scope.askForDeleteComment = function (item) {
        $.confirm({
            title: 'Uyarı!',
            content: "Silmek istediğinize emin misiniz?",
            buttons:
            {
                EVET: {
                    btnClass: 'btn-blue',
                    action: function () { $scope.deleteTaskComment(item); }

                },
                HAYIR: {
                    btnClass: 'btn-red any-other-class',
                    action: function () {
                        iziToast.error({
                            message: 'Silme işleminiz iptal edilmiştir.',
                            position: 'topCenter'
                        });

                    }
                }

            }
        });
    };
    $scope.deleteTaskComment = function (item) {
        fireCustomLoading(true);
        $http({
            method: "POST",
            url: "/Admin/TaskList/DeleteTaskComment",
            headers: { "Content-Type": "Application/json;charset=utf-8" },
            data: { id: item.Id }

        }).then(function (response) {
            var retVal = jQuery.parseJSON(response.data);
            if (retVal.statu === "success") {

                var index = $scope.taskListComment.indexOf(item);
                $scope.taskListComment.splice(index, 1);

                iziToast.success({
                    message: retVal.message,
                    position: 'topCenter'
                });
            } else {
                iziToast.error({
                    //title: 'Hata',
                    message: retVal.message,
                    position: 'topCenter'
                });
                fireCustomLoading(false);

            }
        });
    };

    $(document).ready(function () {

    });



    function setDefaultDate() {
        var today = new Date();
        var dStart = '01/01/' + today.getFullYear();
        $('#iTaskListStartDate').datetimepicker({
            format: "DD/MM/YYYY",
            defaultDate: dStart,
            locale: "tr"
        }).val(dStart);

        $('#iTaskListEndDate').datetimepicker({
            format: 'DD/MM/YYYY',
            locale: 'tr'
        }).val(today.getDate() + '/' + (today.getMonth() + 1) + '/' + today.getFullYear());
    };


    $(window).load(function () {
        setDefaultDate();
    });

}).filter("dateFilter", function () {
    return function (item) {
        if (item != null) {
            return new Date(parseInt(item.substr(6)));
        }
        return "";
    };
});


