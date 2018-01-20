$(document).ready(function(){

});

function addClass() {
    var classPasswd = $("#classPasswd").val();
    var className = $("#className").val();
    if (classPasswd.length >= 4 && className.length >= 4){
        var newPostKey = firebase.database().ref().child('CLASSES').push().key;
        firebase.database().ref('CLASSES/'+newPostKey+"/").set({
            heslo:classPasswd,
            teacherID:getCookie("teacherID"),
            className:className
        }).then(function() {
            console.log('Synchronization succeeded');
            window.location.replace("../../application/showclass/");
            flashMsg("success","Cvicenie bolo ulozene do DB");
        })
            .catch(function(error) {
                console.log('Synchronization failed');
                flashMsg("error","Cvicenie neulozilo do DB");
            });
    }else{
        flashMsg("error","Nazov/Heslo ulohy je priliz kratky");
    }
}