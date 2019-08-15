import { Injectable } from '@angular/core';
import { CanDeactivate } from '@angular/router';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';

@Injectable()
export class PreventUnsavedChangesLoss implements CanDeactivate<MemberEditComponent> {
    canDeactivate(component: MemberEditComponent) {
        if (component.editUser.dirty) {
            return confirm('Are you sure you want to continue? Any unsaved changes will be lost!');
        }

        return true;
    }
}
