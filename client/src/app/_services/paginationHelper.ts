import { HttpClient, HttpParams } from "@angular/common/http";
import { map } from "rxjs/operators";
import { PaginatedResult } from "../_models/pagination";


export function getPaginatedResult<T>(url, params, http: HttpClient) {

    const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>(); // store results in this.

    return http.get<T>(url, { observe: 'response', params }).pipe(
        map(response => {
            // add the response body to paginatedResult body
            paginatedResult.result = response.body;

            // add the response pagination header after JSON deserilizing to paginatedResult pagination
            if (response.headers.get('Pagination') !== null) {
                paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
            }
            return paginatedResult;
        })
    );
}


// seperate function to get the params.
export function getPaginationHeader(pageNumber: number, pageSize: number) {

    let params = new HttpParams();

    params = params.append('pageNumber', pageNumber.toString());
    params = params.append('pageSize', pageSize.toString());

    return params;
}