import { Injectable } from '@angular/core';
import { CanDeactivate } from '@angular/router';
import { Observable } from 'rxjs';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';
import { ConfirmService } from '../_services/confirm.service';

@Injectable({
  providedIn: 'root'
})
export class PreventUnsavedChangesGuard implements CanDeactivate<unknown> {

  constructor(private confirmService: ConfirmService) { }

  // we access the MemberEditComponent here and check whether it's modified
  canDeactivate(component: MemberEditComponent): Observable<boolean> | boolean {
    if (component.editForm.dirty) {
      // js confirm method, return true if user clicks yes, else false
      return this.confirmService.confirm();
    }
    return true;
  }

}
