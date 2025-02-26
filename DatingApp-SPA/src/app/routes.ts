import { Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { ListsComponent } from './lists/lists.component';
import { AuthGuard } from './_guards/auth.guard';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberDetailResolver } from './_resolvers/member-detail.resolver';
import { MemberListResolver } from './_resolvers/member-list.resolver';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { MemberEditResolver } from './_resolvers/member-edit.resolver';
import { PreventUnsavedChangesLoss } from './_guards/prevent-unsaved-changes-loss.guard';
import { ListsResolver } from './_resolvers/lists.resolver';
import { MessagesResolver } from './_resolvers/messages.resolver';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';

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
            {
                path: 'members',
                component: MemberListComponent,
                resolve: {
                    users: MemberListResolver
                }
            },
            {
                path: 'members/:id',
                component: MemberDetailComponent,
                resolve: {
                    user: MemberDetailResolver
                }
            },
            {
                path: 'member/edit',
                component: MemberEditComponent,
                resolve: {
                    user: MemberEditResolver
                },
                canDeactivate: [PreventUnsavedChangesLoss]
            },
            {
                path: 'messages',
                component: MessagesComponent,
                resolve: {
                    messages: MessagesResolver
                }
            },
            {
                path: 'lists',
                component: ListsComponent,
                resolve: {
                    users: ListsResolver
                }
            },
            {
                path: 'admin',
                component: AdminPanelComponent,
                data: {
                    roles: [
                        'Admin', 'Moderator'
                    ]
                }
            }
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
