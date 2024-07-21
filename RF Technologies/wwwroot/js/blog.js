var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    if (dataTable) {
        dataTable.destroy();
    }

    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/blog/getall' },
        "columns": [
            { data: 'title', "width": "15%" },
            { data: 'authorName', "width": "15%" },
            { data: 'publicationDate', "width": "10%" },
            { data: 'category', "width": "20%" },
            {
                data: { id: 'id', lockoutEnd: 'lockoutEnd' },
                "render": function (data) {
                    var today = new Date().getTime();
                    var lockout = new Date(data.lockoutEnd).getTime();

                    if (lockout > today) {
                        return `
                        <div class="text-center">
                            <a onclick="LockUnlock('${data.id}')" class="btn btn-danger text-white" style="cursor:pointer; width:150px;">
                                <i class="bi bi-lock-fill"></i> Lock
                            </a>
                        </div>
                        <div class="text-center">
                            <a href="/Admin/User/RoleManagement?userId=${data.id}" class="btn btn-danger text-white" style="cursor:pointer; width:150px;">
                                <i class="bi bi-pencil-square"></i> Permission
                            </a>
                        </div>`;
                    } else {
                        return `
                        <div class="text-center">
                            <a onclick="LockUnlock('${data.id}')" class="btn btn-success text-white" style="cursor:pointer; width:150px;">
                                <i class="bi bi-unlock-fill"></i> Unlock
                            </a>
                        </div>
                        <div class="text-center">
                            <a href="/Admin/User/RoleManagement?userId=${data.id}" class="btn btn-danger text-white" style="cursor:pointer; width:150px;">
                                <i class="bi bi-pencil-square"></i> Permission
                            </a>
                        </div>`;
                    }
                },
                "width": "25%"
            }
        ]
    });
}

function LockUnlock(id) {
    console.log("LockUnlock called with id:", id);
    $.ajax({
        type: "POST",
        url: '/User/LockUnlock',
        data: JSON.stringify({ id: id }),
        contentType: "application/json",
        success: function (data) {
            console.log("LockUnlock success:", data);
            if (data.success) {
                toastr.success(data.message);
                dataTable.ajax.reload(null, false); // Reload data without resetting paging
            } else {
                toastr.error(data.message);
            }
        },
        error: function (err) {
            console.error("LockUnlock error:", err);
            toastr.error("Something went wrong!");
        }
    });
}
