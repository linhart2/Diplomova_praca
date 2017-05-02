/**
 * Created by Lingo on 29/04/2017.
 */
function showExams() {
    var resultRef = firebase.database().ref('exams/');
    resultRef.on('child_added', function(data) {
        $("<li><a>" + data.key + "</a><a class='close' onclick=return PopupAlert('studenta')><i class='fa fa-window-close-o' aria-hidden=true></i></a></li>").appendTo(".GroupList");
        dict[data.key] = data.val();
        draw(dict);
    });
}
$( document ).ready(function() {
    showExams();
});