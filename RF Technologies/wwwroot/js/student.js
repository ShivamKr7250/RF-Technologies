var dataTable;

$(document).ready(function () {
    const urlParams = new URLSearchParams(window.location.search);
    const status = urlParams.get('status');
    loadDataTable(status);
});

function loadDataTable(status) {
    dataTable = $('#tblBookings').DataTable({
        "ajax": {
            url: '/student/getall?status=' + status
        },
        "columns": [
            { data: 'name', "width": "15%" },
            { data: 'email', "width": "10%" },
            { data: 'domain', "width": "15%" },
            { data: 'collegeName', "width": "10%" },
            { data: 'status', "width": "10%" },
            { data: 'startDate', "width": "10%" },
            { data: 'endDate',  "width": "10%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group">
                    <a href="/student/registrationUpdate?registrationId=${data}" class="btn btn-outline-warning mx-2">
                    <i class="bi bi-pencil-square"></i>
                        Details
                        </a>
                    </div>`
                }
            }
        ]
    });
}