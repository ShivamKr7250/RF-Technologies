var dataTable;

$(document).ready(function () {
    const urlParams = new URLSearchParams(window.location.search);
    const status = urlParams.get('status');
    loadDataTable(status);
});

function loadDataTable(status) {
    dataTable = $('#tblBookings').DataTable({
        "ajax": {
            url: '/student/getall?status=' + status,
            error: function (xhr, error, thrown) {
                console.error('Error loading data: ', error);
                toastr.error('Failed to load data.');
            }
        },
        "columns": [
            { data: 'name', "width": "10%" },
            { data: 'email', "width": "10%" },
            { data: 'domain', "width": "10%" },
            { data: 'collegeName', "width": "10%" },
            { data: 'status', "width": "10%" },
            { data: 'startDate', "width": "10%" },
            { data: 'endDate', "width": "10%" },
            {
                data: 'id',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                        <a href="/student/registrationUpdate?registrationId=${data}" class="btn btn-outline-warning mx-2">
                            <i class="bi bi-pencil-square"></i> Details
                        </a>
                        <a onClick=Delete('/student/delete/${data}') class="btn btn-outline-danger mx-2">
                            <i class="bi bi-trash-fill"></i> Delete
                        </a>
                    </div>`;
                },
                "width": "30%"
            }
        ],
        "language": {
            "emptyTable": "No Registration available"
        }
    });
}

function Delete(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: "DELETE",
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                },
                error: function (xhr, error, thrown) {
                    console.error('Error deleting record: ', error);
                    toastr.error('Failed to delete record.');
                }
            });
        }
    });
}
