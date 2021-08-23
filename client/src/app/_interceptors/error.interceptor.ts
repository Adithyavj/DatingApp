import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { catchError } from 'rxjs/operators';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private router: Router, private toastr: ToastrService) { }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError(error => {
        if (error) {
          switch (error.status) {
            case 400:
              if (error.error.errors) { // 400 Validation error
                const modalStateErrors = [];
                for (const key in error.error.errors) {
                  if (error.error.errors[key]) {
                    // push all the errors into the array
                    modalStateErrors.push(error.error.errors[key]);
                  }
                }
                throw modalStateErrors.flat();
              } else if (typeof (error.error) === 'object') {
                // 400 bad request error           
                this.toastr.error(error.statusText, error.status);
              } else {
                this.toastr.error(error.error, error.status);
              }
              break;
            // in case of 401 Authorization error
            case 401:
              this.toastr.error(error.statusText, error.status);
              break;
            // 404 content not found error
            case 404:
              // redirect to the not found page
              this.router.navigateByUrl('/not-found'); //TODO: Create this
              break;
            // 500 internal server error
            case 500:
              // we need to capture the error we get back from API
              const navigationExtras: NavigationExtras = { state: { error: error.error } };
              // redirect to server error page and passing the error context as state
              this.router.navigateByUrl('/server-error', navigationExtras);
              break;
            default:
              this.toastr.error('Something unexpected went wrong');
              console.log(error);
              break;
          }
        }
        // if it doesn't statisfy the cases in the switch return the error
        return throwError(error);
      })
    )
  }
}
