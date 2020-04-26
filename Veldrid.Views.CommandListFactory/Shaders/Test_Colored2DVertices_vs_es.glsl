#version 310 es

layout(location = 0) in vec2 in_var_POSITION;
layout(location = 1) in vec4 in_var_COLOR;
layout(location = 0) out vec4 out_var_COLOR;

void main()
{
    gl_Position = vec4(in_var_POSITION, 0.0, 1.0);
    out_var_COLOR = in_var_COLOR;
}

