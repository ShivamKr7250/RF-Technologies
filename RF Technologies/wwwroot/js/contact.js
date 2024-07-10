var dataTable;

$(document).ready(function () {
    const urlParams = new URLSearchParams(window.location.search);
    loadDataTable();
});

function loadDataTable(status) {
    dataTable = $('#tblContact').DataTable({
        "ajax": {
            url: '/home/getall'
        },
        "columns": [
            { data: 'name', "width": "10%" },
            { data: 'email', "width": "20%" },
            { data: 'contactNo', "width": "10%" },
            { data: 'subject', "width": "10%" },
            { data: 'message', "width": "60%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group">
                    <a href="/home/registrationUpdate?registrationId=${data}" class="btn btn-outline-warning mx-2">
                    <i class="bi bi-pencil-square"></i>
                        Details
                        </a>
                    </div>`
                }
            }
        ]
    });
}