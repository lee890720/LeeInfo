/**
 * Filter a column on a specific date range. Note that you will likely need 
 * to change the id's on the inputs and the columns in which the start and 
 * end date exist.
 *
 *  @name Date range filter
 *  @summary Filter the table based on two dates in different columns
 *  @author _guillimon_
 *
 *  @example
 *    $(document).ready(function() {
 *        var table = $('#example').DataTable();
 *         
 *        // Add event listeners to the two range filtering inputs
 *        $('#min').keyup( function() { table.draw(); } );
 *        $('#max').keyup( function() { table.draw(); } );
 *    } );
 */

$.fn.dataTable.ext.search.push(
    function (settings, data, dataIndex) {
        var iStart = document.getElementById('date_beg').value;
        var iEnd = document.getElementById('date_end').value;
        var iDateCol = 4;

        iStart = iStart.substring(0, 4) + iStart.substring(5, 7) + iStart.substring(8, 10);
        iEnd = iEnd.substring(0, 4) + iEnd.substring(5, 7) + iEnd.substring(8, 10);

        var iData = data[iDateCol].substring(0, 4) + data[iDateCol].substring(5, 7) + data[iDateCol].substring(8, 10);

        if (iStart === "" && iEnd === "") {
            return true;
        }
        else if (iStart <= iData && iEnd === "") {
            return true;
        }
        else if (iEnd > iData && iStart === "") {
            return true;
        }
        else if (iStart <= iData && iEnd > iData) {
            return true;
        }
        return false;
    }
);