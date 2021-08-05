import { Injectable } from '@angular/core';
import { CanDeactivate } from '@angular/router';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';

@Injectable({
  providedIn: 'root'
})
export class PreventUnsavedChangesGuard implements CanDeactivate<unknown> {

   // we access the MemberEditComponent here and check whether it's modified
  canDeactivate(component: MemberEditComponent): boolean {
    if(component.editForm.dirty){
      // js confirm method, return true if user clicks yes, else false
      return confirm('Are you sure you want to continue? Any unsaved changes will be lost');
    }
    return true;
  }

}
