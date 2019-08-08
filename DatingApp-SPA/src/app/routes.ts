import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { ListsComponent } from './lists/lists.component';
import { AuthGuard } from './_guards/auth.guard';

export const appRoutes: Routes = [
    // Homepage Route
    { path: '', component: HomeComponent },
    
    // =============================================== //
    // Protecting Multiple Routes with One Route Guard //
    // =============================================== //
    {
        path: '',
        runGuardsAndResolvers: 'always',
        canActivate: [AuthGuard],
        children: [
            { path: 'members', component: MemberListComponent },
            { path: 'messages', component: MessagesComponent },
            { path: 'lists', component: ListsComponent },
        ]
    },

    // ================================ //
    // Individual Component Route Guard //
    // ================================ //
    // { path: 'members', component: MemberListComponent, canActivate: [AuthGuard] },
    // { path: 'messages', component: MessagesComponent, canActivate: [AuthGuard] },
    // { path: 'lists', component: ListsComponent, canActivate: [AuthGuard] },

    // The wildcard must be last
    { path: '**', redirectTo: '', pathMatch: 'full' }
];
