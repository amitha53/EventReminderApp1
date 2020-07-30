
$(document).ready(function () {
   
    var events = [];
    var eventSelected = null;

    var OAUTHURL = 'https://accounts.google.com/o/oauth2/auth?';
    var VALIDURL = 'https://www.googleapis.com/oauth2/v1/tokeninfo?access_token=';
    var SCOPE = 'https://www.googleapis.com/auth/userinfo.profile https://www.googleapis.com/auth/userinfo.email';
    var CLIENTID = '296771526171-52i9ubogpr97bl95t2b1lfdfl97t3fo7.apps.googleusercontent.com';
    var REDIRECT = 'https://localhost:44354/User/Home';
    var LOGOUT = 'https://localhost:44354/User/Home';
    var TYPE = 'token';
    var _url = OAUTHURL + 'scope=' + SCOPE + '&client_id=' + CLIENTID + '&redirect_uri=' + REDIRECT + '&response_type=' + TYPE;
    var acToken;
    var tokenType;
    var expiresIn;
    var user;
    var loggedIn = false;

    var error = $('#error').val();
    $('#homepg').css('display', 'none');
    $('#btnlogout').css('display', 'none');
    $('#userDetails').css('display', 'none');
    $('#userWelcome').css('display', 'none');

    if ($('#sessionUserId').val() != null && $('#sessionEmailId').val() != null) {
        $('#modalLogin').hide();
        $('#homepg').css('display', 'block');
        $('#btnlogout').css('display', 'block');
        $('#userDetails').css('display', 'block');
        $('#userWelcome').css('display', 'block');
        var uname = $('#sessionUsername').val();
        $('#user').text(uname);
        FetchEventAndRenderCalender();
        listEvents();
    }
    /*-------------------------Registration scripts-----------------------*/
    $('#register-form-link').click(function () {
        $('#reguserName').val("");
        $('#regdob').val("");
        $('#regphone').val("");
        $('#emailreg').val("");
        $('#regPass').val("");
        $('#regConfirmPass').val("");
    });

    function checkRequiredFeildValidation() {

        if ($('#reguserName').val() == "") {
            $('#errorUsername').text("Enter your Username");
            $('#errorUsername').removeClass("not-error").addClass("error-show");
        }

        if ($('#emailreg').val() == "") {
            $('#errorEmail').text("Enter your EmailID");
            $('#errorEmail').removeClass("not-error").addClass("error-show");
        }

        if ($('#regPass').val() == "") {
            $('#errorPassword').text("Enter your Password");
            $('#errorPassword').removeClass("not-error").addClass("error-show");
        }
        else if ($('#regConfirmPass').val() == "") {
            $('#errorConfirmPass').text("Enter your Password");
            $('#errorConfirmPass').removeClass("not-error").addClass("error-show");
        }
    }
    $("#regphone").change(function () {
        var phone = $('#regphone').val();
        intRegex = /[0-9 -()+]+$/;
        if ((phone.length < 10) || (!intRegex.test(phone))) {
            alert('Please enter a valid phone number.');
            return false;
        }
    });

    $("#emailreg").change(function () {
        var email = $('#emailreg').val();
        var regex = /^([a-zA-Z0-9_\.\-\+])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
        if (!regex.test(email)) {
            alert('Invalid Email Address!!!');
            return false;
        }
    });

    $("#regConfirmPass").change(function () {
        var password = $("#regPass").val();
        var confirmPassword = $("#regConfirmPass").val();
        if (password != confirmPassword) {
            alert("Passwords do not match!!!");
            return false;
        }
    });

    $('#registersubmit').click(function () {
        if ($('#reguserName').val().trim() == "") {
            alert("Enter your Username");
            return;
        }
        if ($('#regphone').val().trim() == "") {
            alert('Enter your Phone');
            return;
        }
        if ($('#emailreg').val().trim() == "") {
            alert('Enter your EmailID');
            return;
        }
        if ($('#regPass').val().trim() == "") {
            alert('Enter your Password');
            return;
        }
        if ($('#regConfirmPass').val().trim() == "") {
            alert('Re-Enter your Password');
            return;
        }
        var regdata = {
            //UserID: $('#reguserId').val(),
            Username: $('#reguserName').val(),
            DOB: $('#regdob').val(),
            Phone: $('#regphone').val(),
            Email: $('#emailreg').val(),
            Password: $('#regPass').val(),
        }
        UserRegister(regdata);
    });
    function UserRegister(regdata) {
        console.log(regdata);
        $.ajax({
            type: "POST",
            url: "/User/Register",
            data: regdata,
            success: function (data) {
                if (data.status) {
                    alert("Registered Successfully");
                    $('#modalLogin').css('display', 'none');
                    $('#homepg').css('display', 'block');
                    $('#btnlogout').css('display', 'block');
                    $('#userDetails').css('display', 'block');
                    $('#userWelcome').css('display', 'block');
                    $('#user').text(data.Username);
                    FetchEventAndRenderCalender();
                    listEvents();
                }
                else {
                    alert("Register Unsuccessfull!!! Email Id already exist!!!");
                }
            },
            error: function () {
                alert('Failed');
            }
        });
    }
/*-------------------------Login scripts-----------------------*/
    $("#loginemail").change(function () {
        var email = $('#loginemail').val();
        var regex = /^([a-zA-Z0-9_\.\-\+])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
        if (!regex.test(email)) {
            alert('Invalid Email Address!!!');
            return false;
        }
    });

    $('#loginsubmit').click(function () {
        if ($('#loginemail').val().trim() == "") {
            alert('Enter your EmailID');
            return;
        }

        if ($('#loginpass').val().trim() == "") {
            alert('Enter your Password');
            return;
        }
        var logindata = {
            Email: $('#loginemail').val(),
            Password: $('#loginpass').val(),
        }
        UserLogin(logindata);
    });
    function UserLogin(logindata) {
        $.ajax({
            type: "POST",
            url: "/User/Login",
            data: logindata,
            success: function (data) {
                if (data.status) {
                    $('#modalLogin').css('display', 'none');
                    $('#homepg').css('display', 'block');
                    $('#btnlogout').css('display', 'block');
                    $('#userDetails').css('display', 'block');
                    $('#userWelcome').css('display', 'block');
                    $('#user').text(data.Username);
                    FetchEventAndRenderCalender();
                    listEvents();
                }
                else {
                    alert("Invalid Email Id or password!!!!", "Failed");
                }

            },
            error: function () {
                alert('Failed');
            }
        });
    }
    /*------------------Google Login scripts--------------*/
          
    $('#btnGoogleLogin').click(function () {
        login();
    });

    function login() {
        var win = window.open(_url, "windowname1", 'width=800, height=600');
        var pollTimer = window.setInterval(function () {
            try {
                console.log(win.document.URL);
                if (win.document.URL.indexOf(REDIRECT) != -1) {
                window.clearInterval(pollTimer);
                    var url = win.document.URL;
                    acToken = gup(url, 'access_token');
                    tokenType = gup(url, 'token_type');
                    expiresIn = gup(url, 'expires_in');

                    win.close();
                    debugger;
                    validateToken(acToken);
                }
            }
            catch (e) {

            }
        }, 500);
    }

    function gup(url, name) {
        namename = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
        var regexS = "[\\#&]" + name + "=([^&#]*)";
        var regex = new RegExp(regexS);
        var results = regex.exec(url);
        if (results == null)
            return "";
        else
            return results[1];
    }

    function validateToken(token) {
        getUserInfo();
            $.ajax(
           {
                url: VALIDURL + token,
                data: null,
                success: function (responseText) {
                },
           });
    }

    function getUserInfo() {
        $.ajax({
            url: 'https://www.googleapis.com/oauth2/v1/userinfo?access_token=' + acToken,
            data: null,
            success: function (resp) {
                user = resp;
                getAccountDetails(user);
                console.log(user);
            }
        });
    }

    function getAccountDetails(user) {
        $.ajax({
            url:'/User/GoogleLogin',
            type: 'POST',
            data: {
                email: user.email,
                name: user.name,
                gender: user.gender,
                lastname: user.lastname,
                location: user.location
            },
            success: function (data) {
                $('#modalLogin').css('display', 'none');
                $('#homepg').css('display', 'block');
                $('#btnlogout').css('display', 'block');
                $('#userDetails').css('display', 'block');
                $('#userWelcome').css('display', 'block');
                $('#user').text(user.name);
                FetchEventAndRenderCalender();
                listEvents();
            },
        });
    }
    /*-----------------------------------------------------*/
/*------------Facebook Login scripts-------------------*/

    // Load the JavaScript SDK asynchronously
    (function (d, s, id) {
        var js, fjs = d.getElementsByTagName(s)[0];
        if (d.getElementById(id)) return;
        js = d.createElement(s); js.id = id;
        js.src = "//connect.facebook.net/en_US/sdk.js";
        fjs.parentNode.insertBefore(js, fjs);
    }(document, 'script', 'facebook-jssdk'));

    $('#btnfacebookLogin').click(function () {
        fbLogin();
    });
    
    window.fbAsyncInit =function () {
        // FB JavaScript SDK configuration and setup
        FB.init({
            appId: '587750735263059', // FB App ID
            cookie: true,  // enable cookies to allow the server to access the session
            xfbml: true,  // parse social plugins on this page
            version: 'v3.2' // use graph api version 2.8
        });

        // Check whether the user already logged in
        FB.getLoginStatus(function (response) {
            if (response.status === 'connected') {
                //display user data
                getFbUserData();
            }
        });
    };

   

    // Facebook login with JavaScript SDK
    function fbLogin() {
        FB.login(function (response) {
            if (response.authResponse) {
                // Get and display the user profile data
                getFbUserData();
            } else {
                document.getElementById('status').innerHTML = 'User cancelled login or did not fully authorize.';
            }
        }, { scope: 'email' });
    }

    // Fetch the user profile data from facebook
    function getFbUserData() {
        FB.api('/me', { locale: 'en_US', fields: 'id,first_name,last_name,email,link,gender,locale,picture' },
            function (response) {
                fbuser = response;
                getUserDetails(fbuser);
                console.log(fbuser);
            });
    }
    function getUserDetails(fbuser) {
        $.ajax({
            url: '/User/FacebookLogin',
            type: 'POST',
            data: {
                email: fbuser.email,
                first_name: fbuser.first_name,
            },
            success: function (data) {
                $('#modalLogin').css('display', 'none');
                $('#homepg').css('display', 'block');
                $('#btnlogout').css('display', 'block');
                $('#userDetails').css('display', 'block');
                $('#userWelcome').css('display', 'block');
                $('#user').text(fbuser.first_name);
                FetchEventAndRenderCalender();
                listEvents();
            },
        });
    }
    /*------------------------------------------------------*/
/*-----------------------HomePage scripts----------------------*/

    //Calender script
    function FetchEventAndRenderCalender() {
        events = [];
        $.ajax({
            type: "GET",
            url: "/User/GetEvents",
            success: function (data) {
                $.each(data, function (i, v) {
                    events.push({
                        eventID: v.EventID,
                        //userID: v.UserID,
                        title: v.Subject,
                        description: v.Description,
                        start: moment(v.StartDate),
                        end: moment(v.EndDate),
                    });
                });
                // console.log(events);
                GenerateCalender(events);
            },
            error: function (error) {
                alert('failed');
            }
        });
    }

    function GenerateCalender(events) {
        console.log(events);
        $('#calender').fullCalendar('destroy');
        $('#calender').fullCalendar({
            contentHeight: 400,
            defaultDate: new Date(),
            timeFormat: 'h(:mm)a',
            header: {
                left: 'prev,next today',
                center: 'title',
                right: 'month,basicWeek,basicDay,agenda'
            },
            eventLimit: true,
            eventColor: '#DD2476',

            events: events,
            eventClick: function (calEvent, jsEvent, view) {
                eventSelected = calEvent;
                $('#myModal #eventTitle').text(calEvent.title);
                var $description = $('<div/>');
                $description.append($('<p/>').html('<b>Start Date:</b>' + calEvent.start.format("DD-MM-YYYY HH:mm a")));
                if (calEvent.end != null) {
                    $description.append($('<p/>').html('<b>End Date:</b>' + calEvent.end.format("DD-MM-YYYY HH:mm a")));
                }
                $description.append($('<p/>').html('<b>Description:</b>' + calEvent.description));
                $('#myModal #pDetails').empty().html($description);
                $('#myModal').modal();
            },
            selectable: true,
            select: function (start, end) {
                selectedEvent = {
                    eventID: 0,
                    title: '',
                    description: '',
                    start: start,
                    end: end,
                };
                openCreateFrorm();
                $('#calender').fullCalendar('unselect');
            },
            editable: true,
            eventDrop: function (events) {
                var date = events.start;
                var inpDate = new Date(date);
                var currDate = new Date();
                var data = {
                    EventID: events.eventID,
                    Subject: events.title,
                    Description: events.description,
                    StartDate: events.start.format("DD-MM-YYYY HH:mm a"),
                    EndDate: events.end.format("DD-MM-YYYY HH:mm a"),
                };
                if (inpDate.setHours(0, 0, 0, 0) >= currDate.setHours(0, 0, 0, 0)) {
                    SaveEvent(data);
                }
                else {
                    if (confirm("The date entered is previous to present date") == true) {
                        SaveEvent(data);
                    }
                    else {
                        alert("Event cannot be created!!!");
                    }
                }
            }
        });
    }
   
    /* --------Calendar edit-------------*/
    $('#btnCalEdit').click(function () {
        openEditForm();
    });
    function openEditForm() {
        if (eventSelected != null) {
            $('#calEventID').val(eventSelected.eventID);
            // $('#hdUserID').val(eventSelected.userID);
            $('#calSubject').val(eventSelected.title);
            $('#calDescription').val(eventSelected.description);
            $('#calStart').val(eventSelected.start.format("DD-MM-YYYY HH:mm"));
            $('#calEnd').val(eventSelected.end.format("DD-MM-YYYY HH:mm"));
        }
        $('#myModal').modal('hide');
        $('#ModalEdit').modal();
    }

    $("#calEnd").change(function () {
        var d1 = moment($('#calStart').val(), "DD-MM-YYYY HH:mm", true);
        var d2 = moment($('#calEnd').val(), "DD-MM-YYYY HH:mm", true);
        if (d1.isValid() == false || d2.isValid() == false) {
            alert("Invalid start or end date");
            return;
        }
        var startDate = moment($('#calStart').val(), "DD-MM-YYYY HH:mm a").toDate();
        var endDate = moment($('#calEnd').val(), "DD-MM-YYYY HH:mm a").toDate();
        if (startDate >= endDate) {
            alert('Start date should not be greater than End date!!!');
            return;
        }
    });
    $('#btnSave').click(function () {
        if ($('#calSubject').val().trim() == "") {
            alert('Subject required');
            return;
        }
        if ($('#calStart').val().trim() == "") {
            alert('Start date required');
            return;
        }
        if ($('#calEnd').val().trim() == "") {
            alert('End date required');
            return;
        }
        var date = moment($('#calStart').val(), "DD-MM-YYYY HH:mm a").toDate();
        var inpDate = new Date(date);
        var currDate = new Date();
        var editdata = {
            EventID: $('#calEventID').val(),
            Subject: $('#calSubject').val().trim(),
            Description: $('#calDescription').val(),
            StartDate: $('#calStart').val(),
            EndDate: $('#calEnd').val()
        }
        if (inpDate.setHours(0, 0, 0, 0) >= currDate.setHours(0, 0, 0, 0)) {
            SaveEvent(editdata);
        }
        else {
            if (confirm("The date entered is previous to present date") == true) {
                SaveEvent(editdata);
            }
            else {
                alert("Event cannot be created!!!");
            }
        }
    });
    function SaveEvent(editdata) {
        $.ajax({
            type: "POST",
            url: "/User/SaveEvent",
            data: editdata,
            success: function (data) {
                if (data.status) {
                    alert("Events updated successfully");
                    FetchEventAndRenderCalender();
                    listEvents();
                    $('#ModalEdit').modal('hide');
                }
            },
            error: function () {
                alert('Failed');
            }
        });
    }
    /* --------Calendar Delete-------------*/
    $('#btnCalDelete').click(function () {
        if (eventSelected != null && confirm('Are you sure?')) {
            $.ajax({
                type: "POST",
                url: "/User/DeleteEvent",
                data: { 'eventID': eventSelected.eventID },
                success: function (data) {
                    if (data.status) {
                        alert("Event deleted!!!");
                        FetchEventAndRenderCalender();
                        listEvents();
                        $('#myModal').modal('hide');
                        
                    }
                },
                error: function () {
                    alert('Failed');
                }
            });
        }
    });
  
    // ListView Script
    function listEvents() {
        $("#listtable tbody tr").empty();
        $.ajax({
            type: "GET",
            url: "/User/GetEvents",
            success: function (data) {
                var items = data;
                console.log(items);
                $.each(data, function (i, item) {
                    var rows = "<tr id=" + item.EventID + ">"
                        + "<td>" + item.EventID + "</td>"
                        + "<td>" + item.Subject + "</td>"
                        + "<td>" + item.Description + "</td>"
                        + "<td>" + item.StartDateStr + "</td>"
                        + "<td>" + item.EndDateStr + "</td>"
                        + "<td>" + "<button class='listEdit'" + ">Edit</button>" + "</td>"
                        + "<td>" + "<button class='listDelete'" + ">Delete</button>" + "</td>"
                        + "</tr>";
                    $('#listtable tbody').append(rows);
                });
               // $('#listtable tbody .listEdit').click(function () {
               //     openEditForm();
               // });

               $('#listtable tbody .listEdit').click(function () {
                    var eveId = $(this).closest('tr').attr("id");
                   openlistEditForm(eveId);
               });

               $('#listtable tbody .listDelete').click(function () {
                    var eventId = $(this).closest('tr').attr("id");
                    if (confirm('Are you sure?') == true) {
                        $.ajax({
                            type: "POST",
                            url: "/User/DeleteEvent",
                            data: { 'eventID': eventId },
                            success: function (data) {
                                alert("Event deleted!!!");
                                $("#" + eventId).remove();
                                listEvents();
                                FetchEventAndRenderCalender();
                            },
                            error: function () {
                                alert('Failed');
                            }
                        });
                    }
               });
                function openlistEditForm(eveId) {
                   $. ajax({
                        type: "POST",
                        url: "/User/Edit",
                        data: { 'id': eveId },
                        success: function (events) {
                            $('#listEventID').val(events.EventID);
                            $('#listSubject').val(events.Subject);
                            $('#listDescription').val(events.Description);
                            $('#listStart').val(events.StartDateStr);
                            $('#listEnd').val(events.EndDateStr);

                            $('#listEdit').modal();
                        },
                        error: function () {
                            alert('Failed');
                        }
                    });
                }
                  $('#btnEdit').click(function () {
                      if ($('#listSubject').val().trim() == "") {
                          alert('Subject required');
                          return;
                      }
                      if ($('#listStart').val().trim() == "") {
                          alert('Start date required');
                          return;
                      }
                      if ($('#listEnd').val().trim() == "") {
                          alert('End date required');
                          return;
                      }
                      var d1 = moment($('#listStart').val(), "DD-MM-YYYY HH:mm:ss", true);
                      var d2 = moment($('#listEnd').val(), "DD-MM-YYYY HH:mm:ss", true);
                      console.log(d1.isValid());
                      if (d1.isValid() == false || d2.isValid() == false) {
                          alert("Invalid start or end date");
                          return;
                      }

                      var startDate = moment($('#listStart').val(), "DD-MM-YYYY HH:mm a").toDate();
                      var endDate = moment($('#listEnd').val(), "DD-MM-YYYY HH:mm a").toDate();
                      if (startDate >= endDate) {
                              alert('Start date should not be greater than End date!!!');
                              return;
                      }
                      var data = {
                          EventID: $('#listEventID').val(),
                          Subject: $('#listSubject').val().trim(),
                          Description: $('#listDescription').val(),
                          StartDate: $('#listStart').val(),
                          EndDate: $('#listEnd').val()
                      }
                      var date = moment($('#listStart').val(), "DD-MM-YYYY HH:mm a").toDate();
                      var inpDate = new Date(date);
                      var currDate = new Date();
                      if (inpDate.setHours(0, 0, 0, 0) >= currDate.setHours(0, 0, 0, 0)) {
                          SaveEvent(data);
                      }
                      else {
                          if (confirm("The date entered is previous to present date") == true) {
                              SaveEvent(data);
                          }
                          else {
                              alert("Event cannot be created!!!");
                          }
                      }
                  });
                  function SaveEvent(data) {
                      $.ajax({
                          type: "POST",
                          url: "/User/SaveEvent",
                          data: data,
                          success: function (data) {
                              if (data.status) {
                                  alert("Events updated successfully");
                                  listEvents();
                                  FetchEventAndRenderCalender();
                                  $('#listEdit').modal('hide');
                              }
                          },
                          error: function () {
                              alert('Failed');
                          }
                      });
                  }
            }
        });
    }

    /*-------------------------------------------------------------*/
    /*-----------------Create Page scripts---------------------------*/

    $('#btncreate').click(function () {
        openCreateFrorm();
    });

    function openCreateFrorm() {
        $('#crteEventID').val("");
        $('#crteSubject').val("");
        $('#crteDescription').val("");
        $('#crteStart').val("");
        $('#crteEnd').val("");
        $('#ModalCreate').modal();
    }
    $("#crteEnd").change(function () {
        var date1 = $('#crteStart').val();
        var date2 = $('#crteEnd').val();
        if (new Date(date1).getDate() == new Date(date2).getDate()) {
            if (new Date(date1).getTime() == new Date(date2).getTime()) {
                alert('Start date should not be greater than or equal to end date');
                return;
            }
        }
        if (new Date(date1) > new Date(date2)) {
            alert('Start date should not be greater than or equal to end date');
            return;
        }

    });
    $('#createBtnSubmit').click(function () {
        if ($('#crteSubject').val().trim() == "") {
            alert('Subject required');
            return;
        }
        if ($('#crteStart').val().trim() == "") {
            alert('Start date required');
            return;
        }
        if ($('#crteEnd').val().trim() == "") {
            alert('End date required');
            return;
        }

        var date = $('#crteStart').val()
        var inpDate = new Date(date);
        var currDate = new Date();
        var createdata = {
            EventID: $('#crteEventID').val(),
            Subject: $('#crteSubject').val().trim(),
            Description: $('#crteDescription').val(),
            StartDate: $('#crteStart').val(),
            EndDate: $('#crteEnd').val()
        }
        if (inpDate.setHours(0, 0, 0, 0) >= currDate.setHours(0, 0, 0, 0)) {
            CreateEvent(createdata);
        }
        else {
            if (confirm("The date entered is previous to present date") == true) {
                CreateEvent(createdata);
            }
            else {
                alert("Event cannot be created!!!")
            }
        }
        
    });
    function CreateEvent(createdata) {
        $.ajax({
            type: "POST",
            url: "/User/CreateEvent",
            data: createdata,
            success: function (createdata) {
                if (createdata.status) {
                    FetchEventAndRenderCalender();
                    listEvents();
                    alert("Event Created Successfully");
                    $('#ModalCreate').modal('hide');
                }
            },
            error: function () {
                alert('Failed');
            }
        });
    }
    /*--------------------------------------------------------------*/
    /*----------------------Logout scripts----------------------------*/

    $('#btnlogout').click(function () {
        logout();

        function logout() {
            $.ajax({
                type: "GET",
                url: "/User/Logout",
                success: function (data) {
                    if (confirm('Are you sure?') == true && data.status) {
                        $('#homepg').css('display', 'none');
                        $('#btnlogout').css('display', 'none');
                        $('#modalLogin').css('display', 'block');
                        $('#userDetails').css('display', 'none');
                        $('#userWelcome').css('display', 'none');
                        //  $('#modalLogin').modal();
                        $('#loginemail').val("");
                        $('#loginpass').val("");
                        $('#reguserName').val("");
                        $('#regdob').val("");
                        $('#regphone').val("");
                        $('#emailreg').val("");
                        $('#regPass').val("");
                        $('#regConfirmPass').val("");
                    }
                },
                error: function () {
                    alert('Failed');
                }
            });
        }
    });
/*--------------------------------------------------------------*/
/*----------------------Forgot Password scripts----------------------------*/
    $('#forgetpassword').click(function () {
        openForm();
    });

    function openForm() {
        $('#resetEmail').val("");
        $('#forgotPassword').modal();
    }
    $('#resetpass').click(function () {
        if ($('#resetEmail').val().trim() == "") {
            alert('Enter your Email ID');
            return;
        }
        var data = {
            Email: $('#resetEmail').val()
        }
        forgotPassword(data);
    });

    function forgotPassword(data) {
        $.ajax({
            type: "POST",
            url: "/User/ForgetPassword",
            data: data,
            success: function (data) {
                if (data.status) {
                    alert("The reset password link is send to you registered Email Id!!!");
                    $('#forgotPassword').modal('hide');
                }
                else {
                    alert("Email Id entered is Invalid");
                }
            },
            error: function () {
                alert("failed");
            }
        });
    }
/*--------------------------------------------------------------*/
/*----------------------User Profile scripts----------------------------*/
    $('#user').click(function () {
        openEditProfileForm();
    });
    function openEditProfileForm() {
        $.ajax({
            type: "POST",
            url: "/User/UserDetails",
            success: function (data) {
                var userdetails = data;
                $('#upUserID').val(userdetails.UserID);
                $('#upUsername').val(userdetails.Username);
                $('#upDob').val(userdetails.DOBStr);
                $('#upPhone').val(userdetails.Phone);
                $('#upEmail').val(userdetails.Email);
                $('#userProfile').modal();

                console.log(userdetails);
            },
            error: function () {
                alert('Failed');
            }
        });
    }

    $("#upPhone").change(function () {
        var phone = $('#upPhone').val();
        intRegex = /[0-9 -()+]+$/;
        if ((phone.length < 10) || (!intRegex.test(phone))) {
            alert('Please enter a valid phone number.');
            return false;
        }

    });

    $("#upDob").change(function () {
        var d1 = moment($('#upDob').val(), "DD-MM-YYYY", true);
        if (d1.isValid() == false) {
            alert("Invalid Date of birth");
            return;
        }
    });

    $("#upEmail").change(function () {
        var email = $('#upEmail').val();
        var regex = /^([a-zA-Z0-9_\.\-\+])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
        if (!regex.test(email)) {
            alert('Invalid Email Address!!!');
            return false;
        }
    });

    $('#btnUserSave').click(function () {
        if ($('#upUsername').val().trim() == "") {
            alert('Username required');
            return;
        }
        if ($('#upPhone').val().trim() == "") {
            alert('Phone number required');
            return;
        }
        if ($('#upEmail').val().trim() == "") {
            alert('Email required');
            return;
        }
        var edituserdata = {
            // UserID: $('#upUserID').val(),
            Username: $('#upUsername').val().trim(),
            DOB: $('#upDob').val(),
            Phone: $('#upPhone').val(),
            Email: $('#upEmail').val()
        }
        SaveUserEvent(edituserdata);
    });
    function SaveUserEvent(edituserdata) {
        $.ajax({
            type: "POST",
            url: "/User/SaveUserDetails",
            data: edituserdata,
            success: function (data) {
                if (data.status) {
                    alert("User Details saved successfully");
                    FetchEventAndRenderCalender();
                    listEvents();
                    $('#user').text(data.Username);
                    $('#userProfile').modal('hide');
                }
            },
            error: function () {
                alert('Failed');
            }
        });
    }
});
