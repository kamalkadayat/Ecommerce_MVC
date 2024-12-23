$(document).ready(function () {
    $('#tblData').DataTable({
        "ajax": {
            url: '/admin/product/getall',
            dataSrc: 'data'
        },
        "columns": [
            { data: 'title' },
          /*  { data: 'description' },*/
            { data: 'isbn' },
            { data: 'author' },
            { data: 'listPrice' },
            { data: 'category.name' }

         
        ]
    });
});
